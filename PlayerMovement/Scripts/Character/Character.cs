using Godot;

public class Character : KinematicBody
{
  [Export] public float speed = 30;
  [Export] public float downwardsVelocity = -5;
  [Export] public float steepAngle = 30 * Mathf.Pi / 180; // 30 degrees in radians
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

    groundingRay.ForceRaycastUpdate();
    
    if(groundingRay.IsColliding() && groundingRay.GetCollisionNormal().AngleTo(Vector3.Up) < steepAngle) // check the ngle is not too steep
    { 
      Transform t = GlobalTransform;
      t.origin.y = groundingRay.GetCollisionPoint().y;
      GlobalTransform = t;

      Vector3 normal = groundingRay.GetCollisionNormal();
      Vector3 cross = GlobalTransform.basis.y.Cross(normal);
      if(cross.Length() > 0.0001f) // If the current player angle is not already at the slope angle
        Rotate(cross.Normalized(), GlobalTransform.basis.y.AngleTo(normal));
      velocity = velocity.Rotated(Vector3.Up.Cross(normal).Normalized(), Vector3.Up.AngleTo(normal)); // Also rotate the velocity
    } 
    else
    {
      MoveAndSlide(new Vector3(0, downwardsVelocity, 0));

      // If player is falling, align him
      Vector3 normal = Vector3.Up;
      Vector3 cross = GlobalTransform.basis.y.Cross(normal);
      if(cross.Length() > 0.0001f) // If the current player angle is not already at the slope angle
        Rotate(cross.Normalized(), GlobalTransform.basis.y.AngleTo(normal));
    }
    MoveAndSlide(velocity);
    if(velocity.Length() > 0) LookAt(GlobalTransform.origin - velocity, GlobalTransform.basis.y);
  }
}
