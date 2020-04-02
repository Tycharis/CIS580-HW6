using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CIS580_HW6.Parallax;
using CIS580_HW6.Particles;
using CIS580_HW6.Sprite;

namespace CIS580_HW6
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HelicopterGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _particleTexture;

        private ParticleSystem _fireParticleSystem;
        private ParticleSystem _grassParticleSystem;
        private ParticleSystem _rainParticleSystem;

        private bool _isFireOn = true;
        private bool _isGrassOn;
        private bool _isRainOn = true;

        public HelicopterGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D spritesheet = Content.Load<Texture2D>("helicopter");
            Texture2D backgroundTexture = Content.Load<Texture2D>("background");
            StaticSprite backgroundSprite = new StaticSprite(backgroundTexture);
            ParallaxLayer backgroundLayer = new ParallaxLayer(this);
            ParallaxLayer playerLayer = new ParallaxLayer(this);

            List<Texture2D> midgroundTextures = new List<Texture2D>
            {
                Content.Load<Texture2D>("midground1"),
                Content.Load<Texture2D>("midground2")
            };

            List<StaticSprite> midgroundSprites = new List<StaticSprite>
            {
                new StaticSprite(midgroundTextures[0]),
                new StaticSprite(midgroundTextures[1], new Vector2(3500, 0))
            };

            ParallaxLayer midgroundLayer = new ParallaxLayer(this);

            List<Texture2D> foregroundTextures = new List<Texture2D>()
            {
                Content.Load<Texture2D>("foreground1"),
                Content.Load<Texture2D>("foreground2"),
                Content.Load<Texture2D>("foreground3"),
                Content.Load<Texture2D>("foreground4")
            };

            List<StaticSprite> foregroundSprites = new List<StaticSprite>();

            for (int i = 0; i < foregroundTextures.Count; i++)
            {
                Vector2 position = new Vector2(i * 3500, 0);
                StaticSprite sprite = new StaticSprite(foregroundTextures[i], position);
                foregroundSprites.Add(sprite);
            }

            ParallaxLayer foregroundLayer = new ParallaxLayer(this);

            player = new Player(spritesheet);

            backgroundLayer.Sprites.Add(backgroundSprite);
            backgroundLayer.DrawOrder = 0;

            midgroundLayer.Sprites.AddRange(midgroundSprites);
            midgroundLayer.DrawOrder = 1;

            playerLayer.Sprites.Add(player);
            playerLayer.DrawOrder = 2;

            foreach (StaticSprite sprite in foregroundSprites)
            {
                foregroundLayer.Sprites.Add(sprite);
            }

            foregroundLayer.DrawOrder = 4;

            backgroundLayer.ScrollController = new PlayerTrackingScrollController(player, 0.1f);
            midgroundLayer.ScrollController = new PlayerTrackingScrollController(player, 0.4f);
            playerLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);
            foregroundLayer.ScrollController = new PlayerTrackingScrollController(player, 1.0f);

            Components.Add(backgroundLayer);
            Components.Add(midgroundLayer);
            Components.Add(playerLayer);
            Components.Add(foregroundLayer);

            // Particle Systems

            _particleTexture = Content.Load<Texture2D>("particle");

            _fireParticleSystem = new ParticleSystem(GraphicsDevice, 1000, _particleTexture, this)
            {
                Emitter = new Vector2(100, 100),
                SpawnPerFrame = 4,

                // Set the SpawnParticle method
                SpawnParticle = (ref Particle particle) =>
                {
                    if (_isFireOn)
                    {
                        MouseState mouse = Mouse.GetState();
                        Random random = new Random();
                        particle.Position = new Vector2(player.Position.X, player.Position.Y);
                        particle.Velocity = new Vector2(
                            MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                            MathHelper.Lerp(0, 100, (float)random.NextDouble()) // Y between 0 and 100
                        );
                        particle.Acceleration = 0.1f * new Vector2(0, (float)-random.NextDouble());
                        particle.Color = Color.Gold;
                        particle.Scale = 1f;
                        particle.Life = 1.0f;
                    }
                },

                // Set the UpdateParticle method
                UpdateParticle = (float deltaT, ref Particle particle) =>
                {
                    particle.Velocity += deltaT * particle.Acceleration;
                    particle.Position += deltaT * particle.Velocity;
                    particle.Scale -= deltaT;
                    particle.Life -= deltaT;
                }
            };

            _grassParticleSystem = new ParticleSystem(GraphicsDevice, 1000, _particleTexture, this)
            {
                Emitter = new Vector2(100, 100),
                SpawnPerFrame = 4,

                // Set the SpawnParticle method
                SpawnParticle = (ref Particle particle) =>
                {
                    if (_isGrassOn)
                    {
                        MouseState mouse = Mouse.GetState();
                        Random random = new Random();
                        particle.Position = new Vector2(player.Position.X, 500);
                        particle.Velocity = new Vector2(
                            MathHelper.Lerp(-50, 50, (float)random.NextDouble()), // X between -50 and 50
                            MathHelper.Lerp(0, 100, (float)random.NextDouble()) // Y between 0 and 100
                        );
                        particle.Acceleration = 0.1f * new Vector2(0, (float)-random.NextDouble());
                        particle.Color = Color.White;
                        particle.Scale = 1f;
                        particle.Life = 1.0f;
                    }
                },

                // Set the UpdateParticle method
                UpdateParticle = (float deltaT, ref Particle particle) =>
                {
                    particle.Velocity += deltaT * particle.Acceleration;
                    particle.Position += deltaT * particle.Velocity;
                    particle.Scale -= deltaT;
                    particle.Life -= deltaT;
                }
            };

            _rainParticleSystem = new ParticleSystem(GraphicsDevice, 1000, _particleTexture, this)
            {
                Emitter = new Vector2(100, 100),
                SpawnPerFrame = 4,

                // Set the SpawnParticle method
                SpawnParticle = (ref Particle particle) =>
                {
                    if (_isRainOn)
                    {
                        MouseState mouse = Mouse.GetState();
                        Random random = new Random();
                        particle.Position = new Vector2(
                            MathHelper.Lerp(0, graphics.GraphicsDevice.Viewport.Width, (float)random.NextDouble()),
                            0
                        );
                        particle.Velocity = new Vector2(
                            0,
                            //MathHelper.Lerp(0, 100, (float)random.NextDouble()) // Y between 0 and 100
                            1000
                        );
                        particle.Acceleration = 0.1f * new Vector2(0, -1.0f);
                        particle.Color = Color.LightBlue;
                        particle.Scale = 0.5f;
                        particle.Life = 2.0f;
                    }
                },

                // Set the UpdateParticle method
                UpdateParticle = (float deltaT, ref Particle particle) =>
                {
                    particle.Velocity += deltaT * particle.Acceleration;
                    particle.Position += deltaT * particle.Velocity;
                    particle.Scale -= deltaT;
                    particle.Life -= deltaT;
                }
            };

            Components.Add(_fireParticleSystem);
            Components.Add(_grassParticleSystem);
            Components.Add(_rainParticleSystem);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update(gameTime);

            _isGrassOn = player.Position.Y >= 500;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
