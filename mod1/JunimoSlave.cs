namespace JunimoIntelliBox
{
    using System;
    using System.Xml.Serialization;
    using JunimoIntelliBox.Animations;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;

    public class JunimoSlave: NPC
    {
        [XmlIgnore]
        private IMonitor monitor;
        [XmlIgnore]
        private InputHelper inputHelper;
        [XmlIgnore]
        private Vector2 movementDirection;
        [XmlIgnore]
        private IJunimoAnimation currentAnimation;

        public JunimoSlave()
        {
        }

        public JunimoSlave(Vector2 position, IMonitor monitor) :
            base(new AnimatedSprite("Characters\\Junimo", 0, 16, 16), position, 0, "JunimoSlave", (LocalizedContentManager)null)
        {

            this.monitor = monitor;
            this.inputHelper = new InputHelper();

            this.ignoreMovementAnimation = true;

            this.movementDirection = Vector2.Zero;

            this.currentAnimation = new JunimoAnimationIdle(this);
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            //this.sprite.Animate(time, 8, 4, 100f);

            this.MovePosition(time, Game1.viewport, location);
            //this.monitor.Log($"JunimoSlave time = {time}, location = {location.Name}", LogLevel.Info);

            //this.monitor.Log($"Junimo udpate(), xVelocity {this.xVelocity} yVelocity {this.yVelocity}", LogLevel.Info);
            
            if (this.currentAnimation != null)
            {
                this.currentAnimation.Play(time);
            }
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (this.isInvisible)
                return;
            // Draw sprite frame;
            b.Draw(this.Sprite.Texture, 
                this.getLocalPosition(Game1.viewport) + new Vector2((float)(this.sprite.Value.spriteWidth * Game1.pixelZoom / 2), (float)((double)this.sprite.Value.spriteHeight * 3.0 / 4.0 * (double)Game1.pixelZoom / Math.Pow((double)(this.sprite.Value.spriteHeight / 16), 2.0)) + (float)this.yJumpOffset - (float)(Game1.pixelZoom * 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), 
                this.GetColor(), 
                this.rotation, 
                new Vector2((float)(this.sprite.Value.spriteWidth * Game1.pixelZoom / 2), (float)((double)(this.sprite.Value.spriteHeight * Game1.pixelZoom) * 3.0 / 4.0)) / (float)Game1.pixelZoom,
                Math.Max(0.2f, this.scale) * (float)Game1.pixelZoom, 
                this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                //Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)(((double)(this.getStandingY() + this.whichJunimoFromThisHut) + (double)this.getStandingX() / 10000.0) / 10000.0))
                (float)this.getStandingY() / 10000f
                );

            // Draw shadow
            if (this.swimming || this.hideShadow)
                return;
            b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2((float)(this.sprite.Value.spriteWidth * Game1.pixelZoom) / 2f, (float)(Game1.tileSize * 3) / 4f - (float)Game1.pixelZoom)), new Rectangle?(Game1.shadowTexture.Bounds), 
                this.GetColor(), 
                0.0f, 
                new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 
                ((float)Game1.pixelZoom + (float)this.yJumpOffset / 40f) * this.scale, 
                SpriteEffects.None, 
                Math.Max(0.0f, (float)this.getStandingY() / 10000f) - 1E-06f);
        }

        public Color GetColor()
        {
            return Color.LightGreen;
        }

        public bool HandleInputEvents(object sender, EventArgsInput e)
        {
            this.movementDirection = this.CalculateMovementBasedOnInput(e);

            this.SetMovementBasedOnDirection(this.movementDirection);

            IJunimoAnimation movementAnimation = this.DecideMovementAnimation(this.movementDirection);

            if (movementAnimation == null)
            {
                this.currentAnimation = new JunimoAnimationIdle(this);
            } else
            {
                this.currentAnimation = movementAnimation;
            }

            return true;
        }

        public override void SetMovingLeft(bool b)
        {
            base.SetMovingLeft(b);

            this.currentAnimation = new JunimoAnimationMoveLeft(this);
        }

        public override void SetMovingRight(bool b)
        {
            base.SetMovingRight(b);

            this.currentAnimation = new JunimoAnimationMoveRight(this);
        }

        public override void SetMovingDown(bool b)
        {
            base.SetMovingDown(b);

            this.currentAnimation = new JunimoAnimationMoveDown(this);
        }

        public override void SetMovingUp(bool b)
        {
            base.SetMovingUp(b);

            this.currentAnimation = new JunimoAnimationMoveUp(this);
        }

        private Vector2 CalculateMovementBasedOnInput(EventArgsInput e)
        {
            if (this.inputHelper.IsOneOfTheseKeysDown(e.Button, Game1.options.moveUpButton))
            {
                return new Vector2(0, -1);
            }
            else if (this.inputHelper.IsOneOfTheseKeysDown(e.Button, Game1.options.moveRightButton))
            {
                return new Vector2(1, 0);
            }
            else if (this.inputHelper.IsOneOfTheseKeysDown(e.Button, Game1.options.moveDownButton))
            {
                return new Vector2(0, 1);
            }
            else if (this.inputHelper.IsOneOfTheseKeysDown(e.Button, Game1.options.moveLeftButton))
            {
                return new Vector2(-1, 0);
            }
            return Vector2.Zero;
        }

        private void SetMovementBasedOnDirection(Vector2 movementDirection)
        {
            float X = movementDirection.X;
            float Y = movementDirection.Y;

            this.Halt();

            if (X > 0)
            {
                this.SetMovingRight(true);
            } else if (X < 0)
            {
                this.SetMovingLeft(true);
            }

            if (Y > 0)
            {
                this.SetMovingDown(true);
            }
            else if (Y < 0)
            {
                this.SetMovingUp(true);
            }
        }

        private IJunimoAnimation DecideMovementAnimation(Vector2 movementDirection)
        {
            float X = movementDirection.X;
            float Y = movementDirection.Y;

            if (Math.Abs(X) > Math.Abs(Y))
            {
                if (X > 0)
                {
                    return new JunimoAnimationMoveRight(this);
                }
                else
                {
                    return new JunimoAnimationMoveLeft(this);
                }
            } else
            {
                if (Y > 0)
                {
                    return new JunimoAnimationMoveDown(this);
                }
                else
                {
                    return new JunimoAnimationMoveUp(this);
                }
            }
        }
    }
}
