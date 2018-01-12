using Microsoft.Xna.Framework;

namespace JunimoIntelliBox.Animations
{
    public class JunimoAnimationMoveUp : JunimoAnimationBase
    {
        public JunimoAnimationMoveUp(JunimoSlave junimo) : base(junimo)
        {
        }

        public override void Play(GameTime time)
        {
            this.junimo.flip = false;
            this.junimo.sprite.Animate(time, 32, 8, 50f);
        }
    }
}
