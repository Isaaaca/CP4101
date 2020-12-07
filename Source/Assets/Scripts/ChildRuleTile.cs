using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu]
public class ChildRuleTile : SiblingRuleTile
{
    public SiblingRuleTile.SibingGroup parentGroup;
    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
                {
                    return other is SiblingRuleTile 
                        && ((other as SiblingRuleTile).sibingGroup == this.sibingGroup
                        || (other as SiblingRuleTile).sibingGroup == this.parentGroup);
                }
            case TilingRule.Neighbor.NotThis:
                {
                    return !(other is SiblingRuleTile
                        && ((other as SiblingRuleTile).sibingGroup == this.sibingGroup
                        || (other as SiblingRuleTile).sibingGroup == this.parentGroup));
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}
