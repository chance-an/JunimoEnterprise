namespace JunimoIntelliBox.Animations
{
    using Microsoft.Xna.Framework;
    class JunimoAnimationIdle : JunimoAnimationBase
    {
        public JunimoAnimationIdle(JunimoSlave _junimo) : base(_junimo)
        {
        }

        public override void Play(GameTime time)
        {
            this.junimo.flip = false;
            this.junimo.sprite.Animate(time, 8, 4, 100f);
        }
    }
}
