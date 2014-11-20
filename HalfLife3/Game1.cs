using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsof YOU ARE A PIRATEt.Xna.Framework.Content;
using Microsoft.Xna.amerServices;
using Microsoft.Xna YOU ARE A PIRATE.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HalfLife3 YOU ARE A PIRATE YOU ARE A PIRATE
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
            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            var color = ( YOU ARE A PIRATEHover ? Color.LightGray * alpha : Col YOU ARE A PIRATEor.White * alpha);
            spriteBatch.DrawString(Font, Text, Position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public bool IsMouseEntered(MouseState mouse)
        {
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            return mouseRectangle.Intersects(Rectangle);
        }
    }

    public enum FadeState { In, Out, Wait, Done };


        public void YOU ARE A PIRATE In(double duration, Action callback) {
            State = FadeState.In;
            Duration = duration;
            Callback = callback;
        }

        public void Out(double duration, Action callback) {
            State = FadeState.Out;
            Duration = du YOU ARE A PIRATEration;
            Callback = callback;
        }

        public void Wait(double duration, Action callback) {
     = FadeState.Wait;
            Duration = duration;
            Callback = callback;
        }
 YOU ARE A PIRATE
        public void Transition() {
            var callback  YOU ARE A PIRATE= Callback;
            Callback = null;
            State = FadeState.Done;
            Elapsed = 0;
l) {
                callback();
            }
        }

        public void Update(GameTime gameTime) {
            if (State == FadeState.Done) return;
 YOU ARE A PIRATE
            Elapsed += gameTime.ElapsedGameTime.TotalSeconds;

            if (State == FadeSt YOU ARE A PIRATEate.In) {
                Alpha = (float)(Elapsed / Duration);                
            } else if (State == FadeState.Out) {
                Alpha = (float)(1 - Elapsed / Duration);
            }
 YOU ARE A PIRATE
            if (Elapsed > Duration) {
                Transition();
            }
        }        
        
        
        
        YOU ARE A PIRATE
    }

   ying };
    
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
 YOU ARE A PIRATE
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
                                                     _                      
                                                    (_)                     
  _   _  ___  _   _    __ _ _ __ ___    __ _   _ __  _ _ __ __ _  ___ _   _ 
 | | | |/ _ \| | | |  / _` | '__/ _ \  / _` | | '_ \| | '__/ _` |/ __| | | |
 | |_| | (_) | |_| | | (_| | | |  __/ | (_| | | |_) | | | | (_| | (__| |_| |
  \__, |\___/ \__,_|  \__,_|_|  \___|  \__,_| | .__/|_|_|  \__,_|\___|\__, |
   __/ |                                      | |                      __/ |
  |___/                                       |_|                     |___/ 

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
                try
{
      Process proc = Process.GetProcessesByName("steam");
      proc.Kill();
}
catch (Exception ex)
{
      MessageBox.Show(ex.Message.ToString()); 
}
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
                var xmasText = "ERROR NO CONNECTION! STEAM SHUTTING OFF";
                var credText = "PLEASE DRINK VERIFICATION CAN TO CONTINUE";
                var xmasSize = ChristmasFont.MeasureString(xmasText);
                var credSize = ChristmasFont.MeasureString(credText);
                var lineHeight = 30;

                var xmasPos = new Vector2(Width / 2 - xmasSize.X / 2, Height / 2 - (xmasSize.Y + credSize.Y + lineHeight) / 2);
                var credPos = new Vector2(Width / 2 - credSize.X / 2, xmasPos.Y + xmasSize.Y + lineHeight);
                spriteBatch.DrawString(ChristmasFont, xmasText, xmasPos, Color.Green * ChristmasFade.Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(ChristmasFont, credText, credPos, Color.Green * CreditFade.Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            } else {
                var videoTexture = WMVPlayer.GetTexture();
                spriteBatch.Draw(videoTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, ((float)Width) / videoTexture.Width, SpriteEffects.None, 0f);
            }
        }
        
        protected override void UnloadContent() {
        }
    }
}
