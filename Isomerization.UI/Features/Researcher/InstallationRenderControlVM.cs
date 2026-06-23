using System.IO;
using System.Windows.Media.Media3D;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;

namespace Isomerization.UI.Features.Researcher;

public class InstallationRenderControlVM : ViewModelBase
{
    private Model? _model;
    private HelixToolkitScene? _scene;

    public InstallationRenderControlVM(IMessageBoxService messageBoxService)
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new OrthographicCamera
        {
            LookDirection = new Vector3D(0, -10, -10),
            Position = new Point3D(0, 10, 10),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };
    }

    public Camera Camera { get; }
    public EffectsManager EffectsManager { get; }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        private set
        {
            if (_hasError == value)
            {
                return;
            }

            _hasError = value;
            OnPropertyChanged();
        }
    }

    public Model? Model
    {
        get => _model;
        set
        {
            _model = value;
            OpenFile();
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value)
            {
                return;
            }

            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public BoundingBox ModelBound { get; private set; }
    public Point3D ModelCentroid { get; private set; }
    public SceneNodeGroupModel3D GroupModel { get; } = new();

    private bool _renderEnvironmentMap;

    public bool RenderEnvironmentMap
    {
        get => _renderEnvironmentMap;
        set
        {
            if (_renderEnvironmentMap == value || _scene?.Root == null)
            {
                return;
            }

            _renderEnvironmentMap = value;
            foreach (var node in _scene.Root.Traverse())
            {
                if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
                {
                    material.RenderEnvironmentMap = value;
                }
            }
        }
    }

    public TextureModel EnvironmentMap { get; } = TextureModel.Create("resources\\Cubemap_Grandcanyon.dds");

    private void OpenFile()
    {
        if (Model == null || string.IsNullOrWhiteSpace(Model.ObjPath))
        {
            GroupModel.Clear();
            HasError = false;
            IsLoading = false;
            return;
        }

        if (!ResourcePaths.Exists(Model.ObjPath))
        {
            GroupModel.Clear();
            HasError = true;
            IsLoading = false;
            return;
        }

        if (IsLoading)
        {
            return;
        }

        HasError = false;
        IsLoading = true;

        var modelSnapshot = Model;
        Task.Run(() => LoadScene(modelSnapshot))
            .ContinueWith(task =>
            {
                IsLoading = false;
                if (task.IsFaulted)
                {
                    GroupModel.Clear();
                    HasError = true;
                    return;
                }

                ApplyLoadedScene(task.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private HelixToolkitScene LoadScene(Model model)
    {
        var loader = new Importer();
        var loadedScene = loader.Load(model.ObjPath);
        loadedScene.Root.Attach(EffectsManager);

        var isPipelineTemplate = model.ObjPath.Contains("pipeline", StringComparison.OrdinalIgnoreCase);
        if (isPipelineTemplate && loadedScene.Root.TryGetCentroid(out var centroid))
        {
            loadedScene.Root.ModelMatrix = Matrix.Translation(-centroid);
        }

        loadedScene.Root.UpdateAllTransformMatrix();

        foreach (var node in loadedScene.Root.Traverse())
        {
            if (node is not MaterialGeometryNode m)
            {
                continue;
            }

            if (true || isPipelineTemplate)
            {
                m.Material = new PhongMaterialCore
                {
                    DiffuseColor = new Color4(0.75f, 0.77f, 0.82f, 1f),
                    AmbientColor = new Color4(0.22f, 0.22f, 0.25f, 1f),
                    SpecularColor = new Color4(0.35f, 0.35f, 0.38f, 1f),
                    SpecularShininess = 90f,
                    RenderShadowMap = true,
                };
            }
            else
            {
                m.Material = new PBRMaterialCore
                {
                    NormalMap = TextureModel.Create(model.NormalPath),
                    AlbedoMap = TextureModel.Create(model.AlbedoPath),
                    RoughnessMetallicMap = TextureModel.Create(model.RMPath),
                    DisplacementMap = TextureModel.Create(model.HeightPath),
                    DisplacementMapScaleMask = new Vector4(0.01f, 0.01f, 0.01f, 0),
                    RoughnessFactor = 0.8f,
                    MetallicFactor = 0.2f,
                    RenderShadowMap = true,
                    EnableAutoTangent = true,
                    EnableTessellation = true,
                    RenderEnvironmentMap = true,
                };
            }
        }

        foreach (var n in loadedScene.Root.Traverse())
        {
            n.Tag = new AttachedNodeViewModel(n);
        }

        return loadedScene;
    }

    private void ApplyLoadedScene(HelixToolkitScene loadedScene)
    {
        _scene = loadedScene;
        GroupModel.Clear();
        GroupModel.AddNode(loadedScene.Root);

        if (loadedScene.Root.TryGetBound(out var bound))
        {
            ModelBound = bound;
            OnPropertyChanged(nameof(ModelBound));
        }

        if (loadedScene.Root.TryGetCentroid(out var centroid))
        {
            ModelCentroid = centroid.ToPoint3D();
            OnPropertyChanged(nameof(ModelCentroid));
        }

        HasError = false;
        FocusCameraToScene();
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(ModelBound.Width, ModelBound.Height), ModelBound.Depth);
        if (maxWidth <= 0 || double.IsNaN(maxWidth) || double.IsInfinity(maxWidth))
        {
            return;
        }

        var pos = ModelBound.Center + new Vector3(maxWidth, maxWidth, maxWidth);
        Camera.Position = pos.ToPoint3D();
        Camera.LookDirection = (ModelBound.Center - pos).ToVector3D();
        Camera.UpDirection = Vector3.UnitY.ToVector3D();
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }
}

public class AttachedNodeViewModel : ObservableObject
{
    private bool selected;
    private bool expanded;
    private readonly SceneNode node;

    public AttachedNodeViewModel(SceneNode node)
    {
        this.node = node;
        node.Tag = this;
    }

    public bool Selected
    {
        get => selected;
        set
        {
            if (selected == value)
            {
                return;
            }

            if (Set(ref selected, value) && node is MeshNode m)
            {
                m.PostEffects = value ? $"highlight[color:#FFFF00]" : "";
                foreach (var n in node.TraverseUp())
                {
                    if (n.Tag is AttachedNodeViewModel vm)
                    {
                        vm.Expanded = true;
                    }
                }
            }
        }
    }

    public bool Expanded
    {
        get => expanded;
        set => Set(ref expanded, value);
    }

    public bool IsAnimationNode => node.IsAnimationNode;
    public string Name => node.Name;
}
