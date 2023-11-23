using Dave.Actions;
using HarmonyLib;

namespace Dave
{
    [HarmonyPatch(typeof(Card))]
    public class CardRenderPatch
    {
        public static Spr red;
        public static Spr black;
        
        [HarmonyPatch("RenderAction")]
        public static void Prefix(G g, State state, ref CardAction action, bool dontDraw)
        {
            Spr? id = null;
            if (action is RandomChoiceActionFactory.RedAction a)
            {
                action = a.action;
                id = red;
            }
            else if (action is RandomChoiceActionFactory.BlackAction b)
            {
                action = b.action;
                id = black;
            }

            if (action.GetIcon(state) == null || dontDraw || id == null)
                return;
            
            var nullable1 = new Rect(-10);
            var key = new UIKey?();
            var rect = nullable1;
            var rectForReticle = new Rect?();
            var rightHint = new UIKey?();
            var leftHint = new UIKey?();
            var upHint = new UIKey?();
            var downHint = new UIKey?();
            var xy = g.Push(key, rect, rectForReticle, rightHint: rightHint, leftHint: leftHint, upHint: upHint, downHint: downHint).rect.xy;
            double x = xy.x;
            double y = xy.y;
            Color? nullable2 = new Color("ffffff");
            Vec? originPx = new Vec?();
            Vec? originRel = new Vec?();
            Vec? scale = new Vec?();
            Rect? pixelRect = new Rect?();
            Color? color = nullable2;
            Draw.Sprite(id, x, y, originPx: originPx, originRel: originRel, scale: scale, pixelRect: pixelRect, color: color);
            g.Pop();
        }
    }
}