public class DefaultKickUpgrade : Upgrade
{
    public override void OnKick(KickPipelineData data)
    {
        if (data.LookingAt != null)
        {
            data.LookingAt.Interact(
                new InteractionPayload()
                {
                    Owner = controller.gameObject,
                    LookDir = MouseLook.Instance.LookData.Direction
                }
            );
            data.CameraShake.GenerateImpulseWithForce(0.1f);
        }
        else if (data.Health != null)
        {
            foreach (var item in data.Health)
            {
                item.Damage(new DamagePayload() { Amount = 10, Owner = controller.gameObject });
            }

            data.CameraShake.GenerateImpulseWithForce(data.Health.Length / 10.0f);
        }
    }
}
