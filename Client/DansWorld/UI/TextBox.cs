using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using DansWorld.GameClient.UI.CustomEventArgs;

namespace DansWorld.GameClient.UI
{
    class TextBox : Control
    {
        public string Text = "";
        public SpriteFont Font;
        public Texture2D BackgroundImage;
        public Color BorderColor = Color.Black;
        public int BorderThickness = 1;
        public bool SpecialCharactersAllowed = false;
        public bool IsPasswordField = false;
        public bool NumbersAllowed = false;
        public bool SpacesAllowed = false;
        public event EventHandler<KeyPressedEventArgs> KeyPressed;
        public int CharacterLimit = 0;

        private Keys[] _previousKeys = null;
        private int _inputTimer = 0;
        private int _repeatTimer = 0;
        private int _blinkTimer = 0;
        private bool _showBlink = false;
        private bool _aKeyDown = false;
        private bool _bKeyDown = false;
        private bool _cKeyDown = false;
        private bool _dKeyDown = false;
        private bool _eKeyDown = false;
        private bool _fKeyDown = false;
        private bool _gKeyDown = false;
        private bool _hKeyDown = false;
        private bool _iKeyDown = false;
        private bool _jKeyDown = false;
        private bool _kKeyDown = false;
        private bool _lKeyDown = false;
        private bool _mKeyDown = false;
        private bool _nKeyDown = false;
        private bool _oKeyDown = false;
        private bool _pKeyDown = false;
        private bool _qKeyDown = false;
        private bool _rKeyDown = false;
        private bool _sKeyDown = false;
        private bool _tKeyDown = false;
        private bool _uKeyDown = false;
        private bool _vKeyDown = false;
        private bool _wKeyDown = false;
        private bool _xKeyDown = false;
        private bool _yKeyDown = false;
        private bool _zKeyDown = false;
        private bool _1KeyDown = false;
        private bool _2KeyDown = false;
        private bool _3KeyDown = false;
        private bool _4KeyDown = false;
        private bool _5KeyDown = false;
        private bool _6KeyDown = false;
        private bool _7KeyDown = false;
        private bool _8KeyDown = false;
        private bool _9KeyDown = false;
        private bool _0KeyDown = false;
        private bool _spaceDown = false;
        private bool _deleteDown = false;
        private bool _tabDown = false;


        private bool _capitalModifier
        {
            get
            {
                return Keyboard.GetState().IsKeyDown(Keys.LeftShift) || 
                       Keyboard.GetState().IsKeyDown(Keys.RightShift) || 
                       Keyboard.GetState().CapsLock;
            }
        }
        private bool _shiftDown
        {
            get
            {
                return Keyboard.GetState().IsKeyDown(Keys.LeftShift) ||
                       Keyboard.GetState().IsKeyDown(Keys.RightShift);
            }
        }
        public TextBox()
        {

        }
        

        public override void Update(GameTime gameTime)
        {
            if (HasFocus)
            {
                KeyboardState kbState = Keyboard.GetState();
                _inputTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (_inputTimer > 30)
                {
                    if (CharacterLimit == 0 || Text.Length < CharacterLimit)
                    {
                        if (kbState.IsKeyDown(Keys.A) && (!_aKeyDown)) { Text += (_capitalModifier ? 'A' : 'a'); }
                        else if (kbState.IsKeyDown(Keys.B) && (!_bKeyDown)) { Text += (_capitalModifier ? 'B' : 'b'); }
                        else if (kbState.IsKeyDown(Keys.C) && (!_cKeyDown)) { Text += (_capitalModifier ? 'C' : 'c'); }
                        else if (kbState.IsKeyDown(Keys.D) && (!_dKeyDown)) { Text += (_capitalModifier ? 'D' : 'd'); }
                        else if (kbState.IsKeyDown(Keys.E) && (!_eKeyDown)) { Text += (_capitalModifier ? 'E' : 'e'); }
                        else if (kbState.IsKeyDown(Keys.F) && (!_fKeyDown)) { Text += (_capitalModifier ? 'F' : 'f'); }
                        else if (kbState.IsKeyDown(Keys.G) && (!_gKeyDown)) { Text += (_capitalModifier ? 'G' : 'g'); }
                        else if (kbState.IsKeyDown(Keys.H) && (!_hKeyDown)) { Text += (_capitalModifier ? 'H' : 'h'); }
                        else if (kbState.IsKeyDown(Keys.I) && (!_iKeyDown)) { Text += (_capitalModifier ? 'I' : 'i'); }
                        else if (kbState.IsKeyDown(Keys.J) && (!_jKeyDown)) { Text += (_capitalModifier ? 'J' : 'j'); }
                        else if (kbState.IsKeyDown(Keys.K) && (!_kKeyDown)) { Text += (_capitalModifier ? 'K' : 'k'); }
                        else if (kbState.IsKeyDown(Keys.L) && (!_lKeyDown)) { Text += (_capitalModifier ? 'L' : 'l'); }
                        else if (kbState.IsKeyDown(Keys.M) && (!_mKeyDown)) { Text += (_capitalModifier ? 'M' : 'm'); }
                        else if (kbState.IsKeyDown(Keys.N) && (!_nKeyDown)) { Text += (_capitalModifier ? 'N' : 'n'); }
                        else if (kbState.IsKeyDown(Keys.O) && (!_oKeyDown)) { Text += (_capitalModifier ? 'O' : 'o'); }
                        else if (kbState.IsKeyDown(Keys.P) && (!_pKeyDown)) { Text += (_capitalModifier ? 'P' : 'p'); }
                        else if (kbState.IsKeyDown(Keys.Q) && (!_qKeyDown)) { Text += (_capitalModifier ? 'Q' : 'q'); }
                        else if (kbState.IsKeyDown(Keys.R) && (!_rKeyDown)) { Text += (_capitalModifier ? 'R' : 'r'); }
                        else if (kbState.IsKeyDown(Keys.S) && (!_sKeyDown)) { Text += (_capitalModifier ? 'S' : 's'); }
                        else if (kbState.IsKeyDown(Keys.T) && (!_tKeyDown)) { Text += (_capitalModifier ? 'T' : 't'); }
                        else if (kbState.IsKeyDown(Keys.U) && (!_uKeyDown)) { Text += (_capitalModifier ? 'U' : 'u'); }
                        else if (kbState.IsKeyDown(Keys.V) && (!_vKeyDown)) { Text += (_capitalModifier ? 'V' : 'v'); }
                        else if (kbState.IsKeyDown(Keys.W) && (!_wKeyDown)) { Text += (_capitalModifier ? 'W' : 'w'); }
                        else if (kbState.IsKeyDown(Keys.X) && (!_xKeyDown)) { Text += (_capitalModifier ? 'X' : 'x'); }
                        else if (kbState.IsKeyDown(Keys.Y) && (!_yKeyDown)) { Text += (_capitalModifier ? 'Y' : 'y'); }
                        else if (kbState.IsKeyDown(Keys.Z) && (!_zKeyDown)) { Text += (_capitalModifier ? 'Z' : 'z'); }
                        else if (kbState.IsKeyDown(Keys.D1) && (!_1KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "!" : "" : NumbersAllowed ? "1" : ""); }
                        else if (kbState.IsKeyDown(Keys.D2) && (!_2KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "\"" : "" : NumbersAllowed ? "2" : ""); }
                        else if (kbState.IsKeyDown(Keys.D3) && (!_3KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "£" : "" : NumbersAllowed ? "3" : ""); }
                        else if (kbState.IsKeyDown(Keys.D4) && (!_4KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "$" : "" : NumbersAllowed ? "4" : ""); }
                        else if (kbState.IsKeyDown(Keys.D5) && (!_5KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "%" : "" : NumbersAllowed ? "5" : ""); }
                        else if (kbState.IsKeyDown(Keys.D6) && (!_6KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "^" : "" : NumbersAllowed ? "6" : ""); }
                        else if (kbState.IsKeyDown(Keys.D7) && (!_7KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "&" : "" : NumbersAllowed ? "7" : ""); }
                        else if (kbState.IsKeyDown(Keys.D8) && (!_8KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "*" : "" : NumbersAllowed ? "8" : ""); }
                        else if (kbState.IsKeyDown(Keys.D9) && (!_9KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? "(" : "" : NumbersAllowed ? "9" : ""); }
                        else if (kbState.IsKeyDown(Keys.D0) && (!_0KeyDown)) { Text += (_shiftDown ? SpecialCharactersAllowed ? ")" : "" : NumbersAllowed ? "0" : ""); }
                        //else if (kbState.IsKeyDown(Keys.Subtract)) { Text += (SpecialCharactersAllowed ? "-" : ""); Space }
                        //else if (kbState.IsKeyDown(Keys.Add)) { Text += (SpecialCharactersAllowed ? "+" : ""); Space }
                        //else if (kbState.IsKeyDown(Keys.OemPeriod)) { Text += (SpecialCharactersAllowed ? "." : ""); Space }
                        else if (kbState.IsKeyDown(Keys.Space) && !_spaceDown) { Text += (SpacesAllowed ? " " : ""); }
                    }

                    if (kbState.IsKeyDown(Keys.Delete) && !_deleteDown && Text.Length > 0) { Text = Text.Substring(0, Text.Length - 1); }
                    if (kbState.IsKeyDown(Keys.Back) && !_deleteDown && Text.Length > 0) { Text = Text.Substring(0, Text.Length - 1); }
                    _inputTimer = 0;
                    _CheckKeysDown();
                    _FireKeyPressedEvent();
                }
            }
            base.Update(gameTime);
        }
        private void _FireKeyPressedEvent()
        {
            KeyboardState kbState = Keyboard.GetState();
            foreach (Keys key in kbState.GetPressedKeys())
            {
                if (_previousKeys != null && !_previousKeys.Contains(key)) 
                    Pressed(new KeyPressedEventArgs(key));
            }
            _previousKeys = kbState.GetPressedKeys();
            
        }
        private void _CheckKeysDown()
        {
            KeyboardState kbState = Keyboard.GetState();
            _aKeyDown = kbState.IsKeyDown(Keys.A);
            _bKeyDown = kbState.IsKeyDown(Keys.B);
            _cKeyDown = kbState.IsKeyDown(Keys.C);
            _dKeyDown = kbState.IsKeyDown(Keys.D);
            _eKeyDown = kbState.IsKeyDown(Keys.E);
            _fKeyDown = kbState.IsKeyDown(Keys.F);
            _gKeyDown = kbState.IsKeyDown(Keys.G);
            _hKeyDown = kbState.IsKeyDown(Keys.H);
            _iKeyDown = kbState.IsKeyDown(Keys.I);
            _jKeyDown = kbState.IsKeyDown(Keys.J);
            _kKeyDown = kbState.IsKeyDown(Keys.K);
            _lKeyDown = kbState.IsKeyDown(Keys.L);
            _mKeyDown = kbState.IsKeyDown(Keys.M);
            _nKeyDown = kbState.IsKeyDown(Keys.N);
            _oKeyDown = kbState.IsKeyDown(Keys.O);
            _pKeyDown = kbState.IsKeyDown(Keys.P);
            _qKeyDown = kbState.IsKeyDown(Keys.Q);
            _rKeyDown = kbState.IsKeyDown(Keys.R);
            _sKeyDown = kbState.IsKeyDown(Keys.S);
            _tKeyDown = kbState.IsKeyDown(Keys.T);
            _uKeyDown = kbState.IsKeyDown(Keys.U);
            _vKeyDown = kbState.IsKeyDown(Keys.V);
            _wKeyDown = kbState.IsKeyDown(Keys.W);
            _xKeyDown = kbState.IsKeyDown(Keys.X);
            _yKeyDown = kbState.IsKeyDown(Keys.Y);
            _zKeyDown = kbState.IsKeyDown(Keys.Z);
            _1KeyDown = kbState.IsKeyDown(Keys.D1);
            _2KeyDown = kbState.IsKeyDown(Keys.D2);
            _3KeyDown = kbState.IsKeyDown(Keys.D3);
            _4KeyDown = kbState.IsKeyDown(Keys.D4);
            _5KeyDown = kbState.IsKeyDown(Keys.D5);
            _6KeyDown = kbState.IsKeyDown(Keys.D6);
            _7KeyDown = kbState.IsKeyDown(Keys.D7);
            _8KeyDown = kbState.IsKeyDown(Keys.D8);
            _9KeyDown = kbState.IsKeyDown(Keys.D9);
            _0KeyDown = kbState.IsKeyDown(Keys.D0);
            _spaceDown = kbState.IsKeyDown(Keys.Space);
            _deleteDown = kbState.IsKeyDown(Keys.Back) || kbState.IsKeyDown(Keys.Delete);
            _tabDown = kbState.IsKeyDown(Keys.Tab);
        }

        public string HideText(string text)
        {
            string hidden = "";
            for (int i = 0; i < Text.Length; i++)
            {
                hidden += "*";
            }
            return hidden;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            Vector2 strDim = Font.MeasureString((IsPasswordField ? HideText(Text) : Text));

            if (BackgroundImage != null)
            {

            }
            else if (BackColor != null)
            {
                spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, 
                                new Rectangle(Destination.Left - BorderThickness, Destination.Top - BorderThickness, 
                                              Destination.Width + BorderThickness * 2, Destination.Height + BorderThickness * 2),
                                BorderColor);
                spriteBatch.Draw(GameClient.DEFAULT_TEXTURE, Destination, BackColor);
            }

            if (!IsPasswordField)
            {
                spriteBatch.DrawString(Font, Text, new Vector2(Location.X + 5, Location.Y + Size.Y / 2 - Font.MeasureString(Text).Y / 2), FrontColor);
            }
            else
            {
                spriteBatch.DrawString(Font, HideText(Text), new Vector2(Location.X + 5, Location.Y + Size.Y / 2 - strDim.Y / 4), FrontColor);
            }

            if (HasFocus)
            {
                Vector2 vecBlink = new Vector2(Location.X + Font.MeasureString(Text).X + 2, Location.Y);
                _blinkTimer++;
                if (_blinkTimer > 50)
                {
                    _showBlink = !_showBlink;
                    _blinkTimer = 0;
                }

                if (_showBlink)
                {
                    spriteBatch.DrawString(Font, "|", new Vector2(Location.X + strDim.X + 7, Location.Y + ((Size.Y / 2) - (Font.MeasureString("|").Y / 2))), FrontColor);
                }
            }
        }
        protected virtual void Pressed(KeyPressedEventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }
    }
}
