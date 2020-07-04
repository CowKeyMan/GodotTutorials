using Godot;

public class CameraRotator : Spatial
{
  public float horizontalSensitivity=1, verticalSensitivity=1;
  [Export] public float minRotationY=-.02f, maxRotationY=1;
  public Vector2 rotation;
  [Export] public Vector3 offset;

  KinematicBody player;
  Camera cam;

  public override void _Ready() 
  {
    rotation = Vector2.Zero;
    player = GetNode<KinematicBody>("../Player"); 
    cam = GetNode<Camera>("Camera"); 
    Input.SetMouseMode(Input.MouseMode.Captured);
  }

  public override void _PhysicsProcess(float delta)
  {
    Transform t = GlobalTransform;
    t.origin = player.Transform.origin + offset;
    t.basis = Basis.Identity;
    GlobalTransform = t;

    Vector2 mouseRelativePosition = InputManager.GetSecondaryAxis();
    if(mouseRelativePosition.x != 0 || mouseRelativePosition.y !=0)
    {
      rotation.x -= mouseRelativePosition.x*delta*verticalSensitivity;
      rotation.y -= mouseRelativePosition.y*delta*horizontalSensitivity;
      if(rotation.y < minRotationY){ rotation.y = minRotationY; }
      else if(rotation.y > maxRotationY){ rotation.y = maxRotationY; }
    }

    this.RotateObjectLocal(Vector3.Up, rotation.x);
    this.RotateObjectLocal(Vector3.Right, rotation.y);

    cam.LookAt(GlobalTransform.origin, Vector3.Up);
  }

}
