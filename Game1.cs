using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Finals_Exam;

public class Game1 : Game
{
    public Random random = new();
    #region Renderer variables
    private GraphicsDeviceManager _graphics;
    public SpriteBatch spriteBatch;
    public Vector2 cameraOffset = Vector2.Zero;
    #endregion

    #region Grid data
    public enum GridState { Empty, Snake, Fruit }
    public List<Primitives> primitives = new();
    public GridState[,] grid;
    public Point gridSize = new(20, 20);
    public int tileSize = 32;
    public Point fruitPos;
    #endregion

    #region Snake data
    public List<Point> snake = new();
    public enum SnakeDirection { Left, Up, Right, Down}
    public SnakeDirection direction = SnakeDirection.Up;
    public float moveTime = 1000;
    public float moveTimer = 0;
    #endregion

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = gridSize.Y * tileSize;
        _graphics.PreferredBackBufferWidth = gridSize.X * tileSize;
        _graphics.ApplyChanges();

        spriteBatch = new(GraphicsDevice);

        grid = new GridState[gridSize.X, gridSize.Y];
        for (int k = 0; k < grid.GetLength(0); k++)
            for (int l = 0; l < grid.GetLength(1); l++)
                grid[k, l] = GridState.Empty;
        
        primitives.Add(new Primitives(GraphicsDevice, Color.Brown));
        primitives.Add(new Primitives(GraphicsDevice, Color.Green));
        primitives.Add(new Primitives(GraphicsDevice, Color.Red));

        snake.Add(new Point(gridSize.X / 2, gridSize.Y / 2));
        snake.Add(new Point(gridSize.X / 2, gridSize.Y / 2));
        snake.Add(new Point(gridSize.X / 2, gridSize.Y / 2));
        snake.Add(new Point(gridSize.X / 2, gridSize.Y / 2));
        snake.Add(new Point(gridSize.X / 2, gridSize.Y / 2));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        // Erm actually this is useless because I LoadContent with my AssetManager.LoadContents
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        moveTimer += gameTime.ElapsedGameTime.Milliseconds;
        if (moveTimer > moveTime)
        {
            moveTimer = 0;
            SnakeMove();
        }


        for (int k = 0; k < grid.GetLength(0); k++)
        {
            for (int l = 0; l < grid.GetLength(1); l++)
            {
                if (snake.Contains(new Point(k, l)))
                {
                    grid[k, l] = GridState.Snake;
                }
                else
                {
                    grid[k, l] = GridState.Empty;
                }
            }
        }
        
        KeyboardState KS = Keyboard.GetState();
        if ((KS.IsKeyDown(Keys.A) || KS.IsKeyDown(Keys.Left)) && direction != SnakeDirection.Right)
        {
            direction = SnakeDirection.Left;
        }
        if ((KS.IsKeyDown(Keys.D) || KS.IsKeyDown(Keys.Right)) && direction != SnakeDirection.Left)
        {
            direction = SnakeDirection.Right;
        }
        if ((KS.IsKeyDown(Keys.W) || KS.IsKeyDown(Keys.Up)) && direction != SnakeDirection.Down)
        {
            direction = SnakeDirection.Up;
        }
        if ((KS.IsKeyDown(Keys.S) || KS.IsKeyDown(Keys.Down)) && direction != SnakeDirection.Up)
        {
            direction = SnakeDirection.Down;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        spriteBatch.Begin();

        // TODO: Add your drawing code here
        for (int k = 0; k < grid.GetLength(0); k++)
        {
            for (int l = 0; l < grid.GetLength(1); l++)
            {
                GridState gridState = grid[k, l];
                
                spriteBatch.Draw(
                    texture: primitives[(int)gridState].texture,
                    destinationRectangle: new Rectangle(k * tileSize, l * tileSize, tileSize, tileSize),
                    color: Color.White);
            }
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }

    public void SnakeMove()
    {
        Point newHead = snake[^1];
        snake.Remove(newHead);
        Point dir = new(0, 0);
        switch (direction)
        {
            case SnakeDirection.Left:
                dir = new(-1, 0);
                break;
            case SnakeDirection.Up:
                dir = new(0, -1);
                break;
            case SnakeDirection.Right:
                dir = new(1, 0);
                break;
            case SnakeDirection.Down:
                dir = new(0, 1);
                break;
        }
        newHead = snake[0] + dir;
        snake.Insert(0, newHead);
    }

    public bool GridCheck(Point gridPos, Predicate<GridState> predicate)
    {
        return predicate.Invoke(grid[gridPos.X, gridPos.Y]);
    }

    public Point GetRandomGridPos()
    {
        return new Point(random.Next(gridSize.X), random.Next(gridSize.Y));
    }

    public Point GetRandomGridPosWithCondition(Predicate<GridState> predicate)
    {
        Point point = GetRandomGridPos();
        while (!GridCheck(point, predicate))
        {
            point = GetRandomGridPos();
        }
        return point;
    }
}
