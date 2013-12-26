using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HalfLife3
{
    public class MenuItem
    {
        public SpriteFont Font;
        public string Text;
        public Vector2 Position;
        public Vector2 Size;
        public Rectangle Rectangle;

        public bool Hover = false;

        public MenuItem(SpriteFont font, string text)
        {
            Font = font;
            Text = text;

            Size = Font.MeasureString(Text);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float alpha) {
            Position = position;
            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            var color = (Hover ? Color.LightGray * alpha : Color.White * alpha);
            spriteBatch.DrawString(Font, Text, Position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public bool IsMouseEntered(MouseState mouse)
        {
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            return mouseRectangle.Intersects(Rectangle);
        }
    }

    public enum FadeState { In, Out, Wait, Done };

    public class Fade {
        public float Alpha = 0;
        public FadeState State = FadeState.Done;
        public double Duration;
        public double Elapsed;
        public Action Callback;

        public Fade() {
        }

        public void In(double duration, Action callback) {
            State = FadeState.In;
            Duration = duration;
            Callback = callback;
        }

        public void Out(double duration, Action callback) {
            State = FadeState.Out;
            Duration = duration;
            Callback = callback;
        }

        public void Wait(double duration, Action callback) {
            State = FadeState.Wait;
            Duration = duration;
            Callback = callback;
        }

        public void Transition() {
            var callback = Callback;
            Callback = null;
            State = FadeState.Done;
            Elapsed = 0;

            if (callback != null) {
                callback();
            }
        }

        public void Update(GameTime gameTime) {
            if (State == FadeState.Done) return;

            Elapsed += gameTime.ElapsedGameTime.TotalSeconds;

            if (State == FadeState.In) {
                Alpha = (float)(Elapsed / Duration);                
            } else if (State == FadeState.Out) {
                Alpha = (float)(1 - Elapsed / Duration);
            }

            if (Elapsed > Duration) {
                Transition();
            }
        }        
    }

    public enum GameState { Intro, Menu, Playing };
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch spriteBatch;
        public float Width;
        public float Height;
        public MouseState MouseState;
        public KeyboardState KeyState;

        public VideoPlayer WMVPlayer;
        public GameState State = GameState.Intro;

        // Intro resources
        public Video ValveVideo;

        // Menu resources
        public Video MenuVideo;
        public SpriteFont TitleFont;
        public SpriteFont MenuFont;
        public SoundEffect ButtonRollover;
        public SoundEffect ButtonClickRelease;
        public List<MenuItem> MenuItems;
        public Fade MenuFade;

        // Ingame resources
        public Video GameVideo;
        public SpriteFont ChristmasFont;
        public Fade ChristmasFade;
        public Fade CreditFade;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TitleFont = Content.Load<SpriteFont>("Font/HalfLife2");
            MenuFont = Content.Load<SpriteFont>("Font/Menu");
            ChristmasFont = Content.Load<SpriteFont>("Font/Christmas");

            ValveVideo = Content.Load<Video>("Video/ValveIntro");
            MenuVideo = Content.Load<Video>("Video/MenuBackground");
            GameVideo = Content.Load<Video>("Video/GameIntro");
            WMVPlayer = new VideoPlayer();

            ButtonRollover = Content.Load<SoundEffect>("Sound/buttonrollover");
            ButtonClickRelease = Content.Load<SoundEffect>("Sound/buttonclickrelease");
            
            StartIntro();
        }

        protected override void Update(GameTime gameTime) {
            MouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) {
                Exit();
            }

            if (State == GameState.Intro && keyboardState.IsKeyDown(Keys.S))
            {
                WMVPlayer.Stop();
                StartMenu();
            }

            if (State == GameState.Intro) {
                UpdateIntro(gameTime);
            } else if (State == GameState.Menu) {
                UpdateMenu(gameTime);
            } else if (State == GameState.Playing) {
                UpdatePlaying(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            Width = Graphics.GraphicsDevice.Viewport.Width;
            Height = Graphics.GraphicsDevice.Viewport.Height;

            // Note: don't use non-premultiplied with XNA 4.0+ fonts
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (State == GameState.Intro) {
                DrawIntro(spriteBatch);
            } else if (State == GameState.Menu) {
                DrawMenu(spriteBatch);
            } else if (State == GameState.Playing) {
                DrawPlaying(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        protected void StartIntro() {
            State = GameState.Intro;
            WMVPlayer.Play(ValveVideo);
        }

        protected void UpdateIntro(GameTime gameTime) {
            if (WMVPlayer.State == MediaState.Stopped) {
                StartMenu();
            }
        }

        protected void DrawIntro(SpriteBatch spriteBatch) {
            var videoTexture = WMVPlayer.GetTexture();
            spriteBatch.Draw(videoTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, ((float)Width) / videoTexture.Width, SpriteEffects.None, 0f);
        }

        protected void StartMenu() {
            State = GameState.Menu;
            WMVPlayer.IsLooped = true;
            WMVPlayer.Play(MenuVideo);

            var menuTexts = new string[] { "NEW GAME", "LOAD GAME", "OPTIONS", "QUIT" };
            MenuItems = new List<MenuItem>();
            foreach (var text in menuTexts) {
                MenuItems.Add(new MenuItem(MenuFont, text));
            }

            MenuFade = new Fade();

            MenuFade.Wait(1, () => {
                MenuFade.In(4, () => {
                    this.IsMouseVisible = true;
                });
            });
        }

        protected void UpdateMenu(GameTime gameTime) {
            MenuFade.Update(gameTime);

            foreach (var item in MenuItems) {
                if (item.IsMouseEntered(MouseState)) {
                    if (MouseState.LeftButton == ButtonState.Pressed) {
                        if (item.Text == "QUIT") {
                            Exit();
                        } else {
                            ButtonClickRelease.Play();
                            StartPlaying();
                        }
                    }
                }
            }
        }

        protected void DrawMenu(SpriteBatch spriteBatch) {
            var videoTexture = WMVPlayer.GetTexture();
            spriteBatch.Draw(videoTexture, Vector2.Zero, null, Color.White *  MenuFade.Alpha, 0f, Vector2.Zero, ((float)Width) / videoTexture.Width, SpriteEffects.None, 0f);

            var titleText = "HALF - LIFE";
            var subText = "3";
            
            var titleSize = TitleFont.MeasureString(titleText);
            var itemSize = MenuFont.MeasureString(MenuItems[0].Text);
            var marginLeft = Width / 8;
            var lineHeight = 10;
            var totalHeight = titleSize.Y + lineHeight + ((itemSize.Y + lineHeight) * MenuItems.Count);

            var color = Color.White * MenuFade.Alpha;

            Vector2 titlePos = new Vector2(marginLeft, Height / 2 - totalHeight / 2);
            spriteBatch.DrawString(TitleFont, titleText, titlePos, color,
                0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Vector2 subPos = new Vector2(titlePos.X + titleSize.X + 5, titlePos.Y);
            spriteBatch.DrawString(TitleFont, subText, subPos, color,
                0, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            var pos = new Vector2(titlePos.X, titlePos.Y + titleSize.Y + lineHeight);
            foreach (var item in MenuItems) {
                if (item.IsMouseEntered(MouseState)) {
                    if (item.Hover == false) {
                        ButtonRollover.Play();
                        item.Hover = true;
                    }
                } else {
                    item.Hover = false;
                }

                item.Draw(spriteBatch, pos, MenuFade.Alpha);
                pos = new Vector2(pos.X, pos.Y + itemSize.Y + lineHeight);
            }
        }

        protected void StartPlaying() {
            State = GameState.Playing;
            this.IsMouseVisible = false;

            WMVPlayer.Stop();

            // Do the Merry Christmas message fade sequence, then
            // play the main game video
            ChristmasFade = new Fade();
            CreditFade = new Fade();

            ChristmasFade.Wait(1, () => {
                ChristmasFade.In(2, () => {
                    WMVPlayer.IsLooped = false;
                    WMVPlayer.Play(GameVideo);
                    CreditFade.In(2, () => {
                        ChristmasFade.Wait(2, () => {
                            ChristmasFade.Out(2, () => {
                                ChristmasFade.Wait(1, null);
                            });

                            CreditFade.Out(2, null);
                        });
                    });
                });
            });
        }

        protected void UpdatePlaying(GameTime gameTime) {
            ChristmasFade.Update(gameTime);
            CreditFade.Update(gameTime);

            // Exit if she watches the video all the way to the end for some mad reason
            if (ChristmasFade.State == FadeState.Done && CreditFade.State == FadeState.Done && WMVPlayer.State == MediaState.Stopped) {
                Exit();
            }
        }

        protected void DrawPlaying(SpriteBatch spriteBatch) {
            if (ChristmasFade.State != FadeState.Done || CreditFade.State != FadeState.Done) {
                var xmasText = "Merry Christmas, Catherine!";
                var credText = "From Jaiden & Emery";
                var xmasSize = ChristmasFont.MeasureString(xmasText);
                var credSize = ChristmasFont.MeasureString(credText);
                var lineHeight = 30;

                var xmasPos = new Vector2(Width / 2 - xmasSize.X / 2, Height / 2 - (xmasSize.Y + credSize.Y + lineHeight) / 2);
                var credPos = new Vector2(Width / 2 - credSize.X / 2, xmasPos.Y + xmasSize.Y + lineHeight);
                spriteBatch.DrawString(ChristmasFont, xmasText, xmasPos, Color.White * ChristmasFade.Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(ChristmasFont, credText, credPos, Color.White * CreditFade.Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            } else {
                var videoTexture = WMVPlayer.GetTexture();
                spriteBatch.Draw(videoTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, ((float)Width) / videoTexture.Width, SpriteEffects.None, 0f);
            }
        }
        
        protected override void UnloadContent() {
        }
    }
}
