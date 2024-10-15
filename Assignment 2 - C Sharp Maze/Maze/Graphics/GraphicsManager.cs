using Maze.Gamestate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maze.Graphics
{
    // Holds data necessary for drawing for all gamestates.
    public class GraphicsManager
    {
        // Drawing component
        public GraphicsDeviceManager graphicsDeviceManager { get; private set; }
        public GraphicsDevice graphicsDevice { get; private set; }
        public SpriteBatch spriteBatch { get; private set; }

        // Window rendering
        public int WINDOW_WIDTH { get; private set; }
        public int WINDOW_HEIGHT { get; private set; }

        // Rendering positioning
        public int PADDING_Y { get; private set; }
        public int RENDERING_AREA_HEIGHT { get; private set; }
        public int PADDING_X { get; private set; }

        // Fonts
        public SpriteFont font1 { get; private set; }
        public int FONT_VERTICAL_SPACING { get; private set; }
        public int TEXT_MARGIN_1 { get; private set; }
        public int TEXT_MARGIN_2 { get; private set; }
        public int TEXT_MARGIN_3 { get; private set; }

        // Textures
        public Texture2D[] wallTextures { get; private set; }
        public Texture2D mainCharacterTexture { get; private set; }
        public Texture2D exitTexture { get; private set; }
        public Texture2D breadCrumbTexture { get; private set; }
        public Texture2D hintTexture { get; private set; }
        public Texture2D rectangleColor { get; private set; }
        public Texture2D background { get; private set; }

        // Efects.
        public RainbowEffect rainbowEffect { get; private set; }
        public TwoColorFade twoColorFadeWalls { get; private set; }

        // Maze Cell Drawing variables
        public int mazeCellPixelDimension { get; set; }
        public Color wallColor1 { get; private set; }
        public Color wallColor2 { get; private set; }

        // Constructor.
        public GraphicsManager(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice)
        {
            // Setup core graphics components.
            this.graphicsDeviceManager = graphicsDeviceManager;
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(this.graphicsDevice);

            // Initialize window rendering properties
            WINDOW_WIDTH = 1920;
            WINDOW_HEIGHT = 1080;

            // Initialize rendering positioning
            PADDING_Y = 80;
            RENDERING_AREA_HEIGHT = WINDOW_HEIGHT - (2 * PADDING_Y);
            PADDING_X = (WINDOW_WIDTH - RENDERING_AREA_HEIGHT) / 2 + 120;

            // Initialize font properties
            FONT_VERTICAL_SPACING = 25;
            TEXT_MARGIN_1 = PADDING_X + RENDERING_AREA_HEIGHT + 15;
            TEXT_MARGIN_2 = 15;
            TEXT_MARGIN_3 = PADDING_X + 15;

            // Initialize Maze Cell Drawing variables
            mazeCellPixelDimension = 0; // Default value, can be changed later
            wallColor1 = new Color(0, 159, 255);
            wallColor2 = new Color(255, 0, 72);
        }

        // Sets back buffer width and height, and configures fullscreen mode.
        public void ConfigureGraphicsDevice()
        {
            // Set back buffer dimensions
            graphicsDeviceManager.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphicsDeviceManager.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphicsDeviceManager.ToggleFullScreen();

            // Apply these settings to the graphics device manager.
            graphicsDeviceManager.ApplyChanges();
        }

        // Loads all textures from content.
        public void LoadGraphicsAssets()
        {
            LoadTextures();
            LoadFonts();
            LoadEffects();
        }

        // Load fonts from Content.
        private void LoadFonts()
        {
            font1 = ContentManagerHandle.Content.Load<SpriteFont>("8Bit");
        }

        // Load effects.
        private void LoadEffects()
        {
            rainbowEffect = new RainbowEffect();
            twoColorFadeWalls = new TwoColorFade(wallColor1, wallColor2);
        }

        // Load textures from Content.
        private void LoadTextures()
        {
            mainCharacterTexture = ContentManagerHandle.Content.Load<Texture2D>("MainCharacter");
            breadCrumbTexture = ContentManagerHandle.Content.Load<Texture2D>("Breadcrumb");
            exitTexture = ContentManagerHandle.Content.Load<Texture2D>("Exit");
            hintTexture = ContentManagerHandle.Content.Load<Texture2D>("CorrectPath");
            background = ContentManagerHandle.Content.Load<Texture2D>("Background");
            rectangleColor = ContentManagerHandle.Content.Load<Texture2D>("RectangleColor");

            wallTextures = new Texture2D[16];
            wallTextures[0] = ContentManagerHandle.Content.Load<Texture2D>("InvalidWall");
            wallTextures[1] = ContentManagerHandle.Content.Load<Texture2D>("Wall1");
            wallTextures[2] = ContentManagerHandle.Content.Load<Texture2D>("Wall2");
            wallTextures[3] = ContentManagerHandle.Content.Load<Texture2D>("Wall3");
            wallTextures[4] = ContentManagerHandle.Content.Load<Texture2D>("Wall4");
            wallTextures[5] = ContentManagerHandle.Content.Load<Texture2D>("Wall5");
            wallTextures[6] = ContentManagerHandle.Content.Load<Texture2D>("Wall6");
            wallTextures[7] = ContentManagerHandle.Content.Load<Texture2D>("Wall7");
            wallTextures[8] = ContentManagerHandle.Content.Load<Texture2D>("Wall8");
            wallTextures[9] = ContentManagerHandle.Content.Load<Texture2D>("Wall9");
            wallTextures[10] = ContentManagerHandle.Content.Load<Texture2D>("Wall10");
            wallTextures[11] = ContentManagerHandle.Content.Load<Texture2D>("Wall11");
            wallTextures[12] = ContentManagerHandle.Content.Load<Texture2D>("Wall12");
            wallTextures[13] = ContentManagerHandle.Content.Load<Texture2D>("Wall13");
            wallTextures[14] = ContentManagerHandle.Content.Load<Texture2D>("Wall14");
            wallTextures[15] = ContentManagerHandle.Content.Load<Texture2D>("Wall15");
        }

        
    }
}
