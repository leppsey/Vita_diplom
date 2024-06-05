using System.Windows.Media.Media3D;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Services;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

namespace Isomerization.UI.Features.Researcher;

public class InstallationRenderControlVM: ViewModelBase
{
    private readonly IMessageBoxService _messageBoxService;
    public Camera Camera { get; }
    public EffectsManager EffectsManager { get; }

    public bool HasError { get; private set; } = false;
    public Model Model
    {
        get => _model;
        set
        {
            _model = value;
            OpenFile();
        }
    }

    public InstallationRenderControlVM(IMessageBoxService messageBoxService)
    {
        _messageBoxService = messageBoxService;
        EffectsManager = new DefaultEffectsManager();
        // Camera = new PerspectiveCamera(){}
        Camera = new OrthographicCamera()
        {
        LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
        Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
        UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
        FarPlaneDistance = 5000,
        NearPlaneDistance = 0.1f
        };
    }
    public bool IsLoading { get; set; }
    public BoundingBox ModelBound { get; set; } = new();
    public Point3D ModelCentroid { get; set; } = default;
    private HelixToolkitScene scene;
    public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

    private bool renderEnvironmentMap = false;
    private Model _model;

    public bool RenderEnvironmentMap
    {
        set
        {
            if (renderEnvironmentMap == value)
            {
                return;
            }
            renderEnvironmentMap = value;
            
            foreach (var node in scene.Root.Traverse())
            {
                if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
                {
                    material.RenderEnvironmentMap = value;
                }
            }
        }
        get => renderEnvironmentMap;
    }

    public TextureModel EnvironmentMap { get; } = TextureModel.Create(
        "C:\\Users\\aspir\\Desktop\\helix-toolkit-master\\Images\\EnvironmentMaps\\Cubemap_Grandcanyon.dds");

    private void OpenFile()
    {
        HasError = false;
            if (IsLoading || string.IsNullOrEmpty(Model.ObjPath))
            {
                HasError = true;
                return;
            }
            var syncContext = SynchronizationContext.Current;
            IsLoading = true;

            Task.Run(async () =>
            {
                // await Task.Delay(5000);
                var loader = new Importer();
                var scene = loader.Load(Model.ObjPath);
                scene.Root.Attach(EffectsManager); // Pre attach scene graph
                scene.Root.UpdateAllTransformMatrix();
                if (scene.Root.TryGetBound(out var bound))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelBound = bound; }, null);
                }
                if (scene.Root.TryGetCentroid(out var centroid))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelCentroid = centroid.ToPoint3D(); }, null);
                }
                return scene;
            }).ContinueWith((result) =>
            {
                if (result.IsCompletedSuccessfully)
                {
                    scene = result.Result;
                    GroupModel.Clear();
                    if (scene != null)
                    {
                        if (scene.Root != null)
                        {
                            foreach (var node in scene.Root.Traverse())
                            {
                                if (node is MaterialGeometryNode m)
                                {
                                    m.Material = new PBRMaterialCore()
                                    {
                                        NormalMap = TextureModel.Create(Model.NormalPath),
                                        AlbedoMap = TextureModel.Create(Model.AlbedoPath),
                                        RoughnessMetallicMap = TextureModel.Create(Model.RMPath),
                                        DisplacementMap = TextureModel.Create(Model.HeightPath),
                                        DisplacementMapScaleMask = new Vector4(0.01f,0.01f,0.01f,0),
                                        RoughnessFactor = 0.8f,
                                        MetallicFactor = 0.2f,                
                                        RenderShadowMap = true,
                                        EnableAutoTangent = true,
                                        EnableTessellation = true,
                                        RenderEnvironmentMap = true,
                                    };
                                }
                            }
                        }

                        GroupModel.AddNode(scene.Root);
                        foreach (var n in scene.Root.Traverse())
                        {
                            n.Tag = new AttachedNodeViewModel(n);
                        }
                    }
                }
                else if (result.IsFaulted && result.Exception != null)
                {
                    HasError = true;
                    // _messageBoxService.Show("result.Exception.Message", "Ошибка!");
                }
                IsLoading = false;
                FocusCameraToScene();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(ModelBound.Width, ModelBound.Height), ModelBound.Depth);
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
    private bool selected = false;
    public bool Selected
    {
        set
        {
            if (selected ==  value)
            {
                return;
            }
            
            if (Set(ref selected, value))
            {
                if(node is MeshNode m)
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
        get => selected;
    }

    private bool expanded = false;
    public bool Expanded
    {
        set => Set(ref expanded, value);
        get => expanded;
    }

    public bool IsAnimationNode { get => node.IsAnimationNode; }

    public string Name { get => node.Name; }

    private SceneNode node;

    public AttachedNodeViewModel(SceneNode node)
    {
        this.node = node;
        node.Tag = this;
    }
}
