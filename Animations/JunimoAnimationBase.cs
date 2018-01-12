using Microsoft.Xna.Framework;

namespace JunimoIntelliBox.Animations
{
    public abstract class JunimoAnimationBase : IJunimoAnimation
    {
        protected JunimoIntelliBox.JunimoSlave junimo;
        public JunimoAnimationBase(JunimoIntelliBox.JunimoSlave _junimo)
        {
            this.junimo = _junimo;
        }

        public abstract void Play(GameTime time);
    }
}
