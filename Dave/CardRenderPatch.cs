using Dave.Actions;
using HarmonyLib;

namespace Dave
{
    [HarmonyPatch(typeof(Card))]
    [HarmonyPatch("RenderAction")]
    public class CardRenderPatch
    {
        public static Spr blue;
        public static Spr orange;
        
        public static void Prefix(G g, State state, ref CardAction action, bool dontDraw)
        {
            Spr? id = null;
            if (action is RandomChoiceActionFactory.BlueAction a)
            {
                action = a.action;
                id = blue;
            }
            else if (action is RandomChoiceActionFactory.OrangeAction b)
            {
                action = b.action;
                id = orange;
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