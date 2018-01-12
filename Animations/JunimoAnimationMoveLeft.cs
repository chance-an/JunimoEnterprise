using Microsoft.Xna.Framework;

namespace JunimoIntelliBox.Animations
{
    public class JunimoAnimationMoveLeft : JunimoAnimationBase
    {
        public JunimoAnimationMoveLeft(JunimoSlave junimo) : base(junimo)
        {
        }

        public override void Play(GameTime time)
        {
            this.junimo.flip = true;
            this.junimo.sprite.Animate(time, 16, 8, 50f);
        }
    }
}
