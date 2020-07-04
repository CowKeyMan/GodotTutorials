using Godot;

public class Character : KinematicBody
{
  [Export] public float speed = 30;
  [Export] public float downwardsVelocity = -5;
  Vector3 NoVertical;
  public RayCast groundingRay;
  Camera cam;

  public Character() { NoVertical = new Vector3(1,0,1); }

  public override void _Ready()
  {
    cam = GetNode<Camera>("../Camera/Camera");
    groundingRay = GetNode<RayCast>("GroundingRay");
  }

  public Vector3 GetAxisDirection()
  { 
    return (
        (
         cam.GlobalTransform.basis.x * InputManager.GetAxisHorizontal() 
         + cam.GlobalTransform.basis.z * InputManager.GetAxisVertical()
        )
        * NoVertical).Normalized(); 
  }

  public override void _PhysicsProcess(float delta)
  {
    Vector3 velocity = GetAxisDirection() * speed;
    if(velocity.Length() > 0) LookAt(GlobalTransform.origin - velocity, Vector3.Up);

    groundingRay.ForceRaycastUpdate();
    Transform t = GlobalTransform;
    if(groundingRay.IsColliding())
    { 
      t.origin.y = groundingRay.GetCollisionPoint().y;
      GlobalTransform = t;
    } 
    else
    {
      MoveAndSlide(new Vector3(0, downwardsVelocity, 0));
    }
    MoveAndSlide(velocity);
      
  }
}
