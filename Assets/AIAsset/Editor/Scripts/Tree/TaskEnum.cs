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
        Selector,
        Sequence
    }

    public enum EDecorateTask
    {
        Root,
        Service,
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
