using Microsoft.Xna.Framework;

namespace JunimoIntelliBox.Animations
{
    public class JunimoAnimationMoveRight : JunimoAnimationBase
    {
        public JunimoAnimationMoveRight(JunimoSlave junimo) : base(junimo)
        {
        }

        public override void Play(GameTime time)
        {
            this.junimo.flip = false;
            this.junimo.sprite.Value.Animate(time, 16, 8, 50f);
        }
    }
}

