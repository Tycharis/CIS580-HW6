using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CIS580_HW6.Particles
{
    /// <summary>
    /// A delegate for spawning particles
    /// </summary>
    /// <param name="particle">The particle to spawn</param>
    public delegate void ParticleSpawner(ref Particle particle);

    /// <summary>
    /// A delegate for updating particles
    /// </summary>
    /// <param name="deltaT">The seconds elapsed between frames</param>
    /// <param name="particle">The particle to update</param>
    public delegate void ParticleUpdater(float deltaT, ref Particle particle);

    /// <summary>
    /// A class representing a particle system 
    /// </summary>
    public class ParticleSystem : DrawableGameComponent
    {
        /// <summary>
        /// The collection of particles 
        /// </summary>
        private Particle[] _particles;

        /// <summary>
        /// The texture this particle system uses 
        /// </summary>
        private Texture2D _texture;

        /// <summary>
        /// The SpriteBatch this particle system uses
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// A random number generator used by the system 
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// The next index in the particles array to use when spawning a particle
        /// </summary>
        private int _nextIndex = 0;

        /// <summary>
        /// Holds a delegate to use when spawning a new particle
        /// </summary>
        public ParticleSpawner SpawnParticle { get; set; }

        /// <summary>
        /// Holds a delegate to use when updating a particle 
        /// </summary>
        /// <param name="particle"></param>
        public ParticleUpdater UpdateParticle { get; set; }

        /// <summary>
        /// The emitter location for this particle system 
        /// </summary>
        public Vector2 Emitter { get; set; }

        /// <summary>
        /// The rate of particle spawning 
        /// </summary>
        public int SpawnPerFrame { get; set; }

        /// <summary>
        /// Constructs a new particle system 
        /// </summary>
        /// <param name="graphicsDevice">The graphics device</param>
        /// <param name="size">The maximum number of particles in the system</param>
        /// <param name="texture">The texture of the particles</param> 
        public ParticleSystem(GraphicsDevice graphicsDevice, int size, Texture2D texture, Game game) : base (game)
        {
            _particles = new Particle[size];
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _texture = texture;
        }

        /// <summary> 
        /// Updates the particle system, spawining new particles and 
        /// moving all live particles around the screen 
        /// </summary>
        /// <param name="gameTime">A structure representing time in the game</param>
        public override void Update(GameTime gameTime)
        {
            // Make sure our delegate properties are set
            if (SpawnParticle == null || UpdateParticle == null) return;

            // Part 1: Spawn Particles

            for (int i = 0; i < SpawnPerFrame; i++)
            {
                // Spawn Particle at nextIndex
                SpawnParticle(ref _particles[_nextIndex]);

                // Advance the index 
                _nextIndex++;
                if (_nextIndex > _particles.Length - 1)
                {
                    _nextIndex = 0;
                }
            }

            // Part 2: Update Particles

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < _particles.Length; i++)
            {
                // Skip any "dead" particles
                if (_particles[i].Life <= 0) continue;

                // TODO: Update the individual particles
                UpdateParticle(deltaT, ref _particles[i]);
            }
        }

        /// <summary>
        /// Draw the active particles in the particle system
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            // Iterate through the particles
            for (int i = 0; i < _particles.Length; i++)
            {
                // Skip any "dead" particles
                if (_particles[i].Life <= 0) continue;

                // Draw the individual particles
                _spriteBatch.Draw(_texture, _particles[i].Position, null, _particles[i].Color, 0f, Vector2.Zero, _particles[i].Scale, SpriteEffects.None, 0);
            }

            _spriteBatch.End();
        }
    }
}
