using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GravityTesting
{
    /*Resources/Links:
     1. http://buildnewgames.com/gamephysics/ is the website that this comes from
     2. https://www.youtube.com/channel/UCF6F8LdCSWlRwQm_hfA2bcQ Good videos on coding with math
     */

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _box;
        private int _screenHeight;
        private Vector2 _position = new Vector2(200, 0f);
        //private float _x = 200f;//This is the objects position in the x direction
        //private float _y = 0f;//This is the objects position in the y direction
        private float _velocityY = 0f; //This is the objects velocity only in the y-direction
        private float _accelerationY = 0f;//This is the objects acceleration only in the y-direction
        private float _mass = 0.1f;    // Ball mass in kg
        private float _radius = 50f;     // Ball radius in cm; or pixels.
        private float _deltaTime = 0.02f;  // Time step in the units of seconds

        /*This is the amount(constant) of gravitational pull that earth has.
          This number represents the rate that objects accelerate towards earth at 
          a rate of 9.807 m/s^2(meters/second squared) due to the force of gravity.
         */
        private float _gravity = 9.807f;

        /* Coefficient of restitution ("bounciness"). Needs to be a negative number for flipping the direction of travel (velocity Y) to move the ball 
           in the opposition direction when it hits a surface. This is what simulates the bouncing effect of an object hitting another object.
        */
        private float _restitutionCoeffecient = -0.5f;

        private float _density = 1.2f;  // Density of air. Try 1000 for water.
        private float _dragCoeffecient = 0.47f; // Coeffecient of drag for a ball

        /* Frontal area of the ball; divided by 10000 to compensate for the 1px = 1cm relation
           frontal area of the ball is the area of the ball as projected opposite of the direction of motion.
           In other words, this is the "silhouette" of the ball that is facing the "wind" (since this variable is used for air resistance calculation).
           It is the total area of the ball that faces the wind. In short: this is the area that the air is pressing on.
           http://www.softschools.com/formulas/physics/air_resistance_formula/85/
        */
        private float _A = 0f; 

        public Main()
        {
            _A = (float)Math.PI * _radius * _radius / 50000f;
            _graphics = new GraphicsDeviceManager(this);
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
            _graphics.PreferredBackBufferHeight = _graphics.PreferredBackBufferHeight + 200;
            _graphics.ApplyChanges();

            _screenHeight = _graphics.PreferredBackBufferHeight;

            var screenCenterX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            var screenCenterY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            var halfWidth = _graphics.GraphicsDevice.Viewport.Width / 2;
            var halfHeight = _graphics.GraphicsDevice.Viewport.Height / 2;

            Window.Position = new Point(screenCenterX - halfWidth, screenCenterY - halfHeight);

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _box = Content.Load<Texture2D>(@"Graphics\OrangeBox");
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var allForces = 0f;//I think fy stands for force Y

            //Add the weight force, which only affects the y-direction (because that's the direction gravity is pulling from)
            //https://www.wikihow.com/Calculate-Force-of-Gravity
            allForces += _mass * _gravity;

            /*Add the air resistance force. This would affect both X and Y directions, but we're only looking at the y-axis in this example.
                Things to note:
                1. Multiplying 0.5 is the same as dividing by 2.  The original well known equation in the link below divides by 2 instead of \
                   multiplying by 0.5.
                2. Mutiplying the -1 constant is to represent the opposite direction that the wind is traveling compared to the direction 
                   the object is traveling
                3. Multiplying _velocityY * _velocityY is the same thing as _velocity^2 which is in the well known equation in the link below
            */
            http://www.softschools.com/formulas/physics/air_resistance_formula/85/
            allForces += Util.CalculateDragForceOnObject(_density, _dragCoeffecient, _A, _velocityY);

            /* Verlet integration for the y-direction
             * This is the amount the ball will be moving in this frame based on the ball's current velocity and acceleration. 
             * Part 1: https://www.youtube.com/watch?v=3HjO_RGIjCU
             * Part 2: https://www.youtube.com/watch?v=pBMivz4rIJY
             * Refer to C++ code sample and the velocity_verlet() function
             *      https://leios.gitbooks.io/algorithm-archive/content/chapters/physics_solvers/verlet/verlet.html
            */
            var predictedDeltaY = Util.IntegrateVelocityVerlet(_velocityY, _deltaTime, _accelerationY);

            // The following calculation converts the unit of measure from cm per pixel to meters per pixel
            _position.Y += predictedDeltaY * 100f;
            //_y += predictedDeltaY * 100f;//OLD

            /*Update the acceleration in the Y direction to take in effect all of the added forces as well as the mass
             Find the new acceleration of the object in the Y direction by solving for A(Accerlation) by dividing all
             0f the net forces by the mass of the object.  This is one way to find out the acceleration.
             */
            var newAccelerationY = allForces / _mass;

            var averageAccelerationY = Util.Average(new[] { newAccelerationY, _accelerationY });

            _velocityY += averageAccelerationY * _deltaTime;

            //Let's do very simple collision detection
            if (_position.Y + _radius > _screenHeight && _velocityY > 0)
            {
                // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                _velocityY *= _restitutionCoeffecient;

                // Move the ball back a little bit so it's not still "stuck" in the wall
                //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                _position.Y = _screenHeight - _radius;
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_box, new Vector2(_position.X, _position.Y), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}