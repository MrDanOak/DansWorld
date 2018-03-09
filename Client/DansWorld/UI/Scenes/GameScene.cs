using DansWorld.Common.GameEntities;
using DansWorld.Common.Net;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.UI.Scenes
{
    class GameScene : BaseScene
    {
        private List<CharacterSprite> characterSprites;
        private ContentManager _content;
        private GameClient _gameClient;


        public List<Character> Characters
        {
            get
            {
                List<Character> ret = new List<Character>();
                foreach (CharacterSprite sprite in characterSprites)
                {
                    ret.Add(sprite.Character);
                }
                return ret;
            }
        }

        public GameScene(GameClient gameClient)
        {
            Controls = new List<Control>();
            _gameClient = gameClient;
        }

        public override void Initialise(ContentManager Content)
        {
            characterSprites = new List<CharacterSprite>();
            _content = Content;
        }

        public void AddCharacter(Character character)
        {
            CharacterSprite sprite = new CharacterSprite()
            {
                Name = character.Name + "sprite",
                IsVisible = true,
                BaseTexture = _content.Load<Texture2D>("Images/Characters/base"),
                SpriteWidth = 48,
                SpriteHeight = 48,
                Size = new Point(48, 48),
                Location = new Point(character.X, character.Y),
                FrontColor = Color.White,
                Character = character,
                InGame = true
            };
            characterSprites.Add(sprite);
        }

        public void RemoveCharacter(Character character)
        {
            CharacterSprite toRemove = new CharacterSprite();
            foreach (CharacterSprite sprite in characterSprites)
            {
                if (character == sprite.Character)
                {
                    toRemove = sprite;
                }
            }
            characterSprites.Remove(toRemove);
        }

        public void ClearCharacters()
        {
            characterSprites = new List<CharacterSprite>();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (CharacterSprite characterSprite in characterSprites)
            {
                characterSprite.Draw(gameTime, spriteBatch);
            }
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            bool moved = false;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) { characterSprites[0].Character.X -= 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.LEFT); moved = true; }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { characterSprites[0].Character.Y += 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.DOWN); moved = true; }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) { characterSprites[0].Character.X += 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.RIGHT); moved = true; }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) { characterSprites[0].Character.Y -= 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.UP); moved = true; }
            if (moved)
            {
                PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.MOVE);
                pb = pb.AddInt(characterSprites[0].Character.X)
                    .AddInt(characterSprites[0].Character.Y)
                    .AddByte((byte)characterSprites[0].Character.Facing)
                    .AddInt(_gameClient.CharacterID);
                GameClient.NetClient.Send(pb.Build());
            }

            foreach (CharacterSprite characterSprite in characterSprites)
            {
                characterSprite.Update(gameTime);
            }
            base.Update(gameTime);
        }
    }
}
