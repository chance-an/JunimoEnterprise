using Microsoft.Xna.Framework;

namespace JunimoIntelliBox.Animations
{
    public class JunimoAnimationMoveDown : JunimoAnimationBase
    {
        public JunimoAnimationMoveDown(JunimoSlave junimo) : base(junimo)
        {
        }

        public override void Play(GameTime time)
        {
            this.junimo.flip = false;
            this.junimo.sprite.Animate(time, 0, 8, 50f);
        }
    }
}
