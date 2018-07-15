namespace BT
{
    public enum TaskType
    {
        CompositeTask,
        DecorateTask,
        ActionTask
    }

    public enum ECompositeTask
    {
        Service,
        Selector,
        Sequence
    }

    public enum EDecorateTask
    {
        Root,
        Bool,
        DistanceDecorate
    }

    public enum EActionTask
    {
        CharacterDeadAction,
        AStarTrackAtion,
        RoundingTrackAction,
        RushTrackAtion,
        AttackAction
    }
}
