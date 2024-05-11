using System.Windows.Media.Media3D;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Isomerization.UI.Services;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;

namespace Isomerization.UI.Features.Researcher;

public class InstallationRenderControlVM: ViewModelBase
{
    private readonly IMessageBoxService _messageBoxService;
    public Camera Camera { get; }
    public EffectsManager EffectsManager { get; }

    public InstallationRenderControlVM(IMessageBoxService messageBoxService)
    {
        _messageBoxService = messageBoxService;
        EffectsManager = new DefaultEffectsManager();
        Camera = new OrthographicCamera()
        {
            LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
            Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
            UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };
        OpenFile();
    }
    public bool IsLoading { get; set; }
    public BoundingBox ModelBound { get; set; } = new();
    public Point3D ModelCentroid { get; set; } = default;
    private HelixToolkitScene scene;
    public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

    private bool renderEnvironmentMap = false;
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
    
    private void OpenFile()
        {
            if (IsLoading)
            {
                return;
            }
            string path = "C:\\Users\\aspir\\Desktop\\Plantlinker\\MAYTEST\\Plantlinker\\models\\model338.ifc";
            if (path == null)
            {
                return;
            }
            var syncContext = SynchronizationContext.Current;
            IsLoading = true;

            Task.Run(() =>
            {
                var loader = new Importer();
                return scene = loader.Load(path);
            }).ContinueWith((result) =>
            {
                IsLoading = false;
                if (result.IsCompleted)
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
                                    if (m.Material is PBRMaterialCore pbr)
                                    {
                                        pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                    else if (m.Material is PhongMaterialCore phong)
                                    {
                                        phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
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
                    _messageBoxService.Show("result.Exception.Message", "Ошибка!");
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(ModelBound.Width, ModelBound.Height), ModelBound.Depth);
        var pos = ModelBound.Center + new Vector3(0, 0, maxWidth);
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
