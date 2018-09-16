using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: reconsider all of this.  It all feels like the wrong design direction
public enum ECommandType
{
    MoveAttack,
    MoveToTarget,
    MoveToTargetCursor,
    AttackTarget,
    FollowTarget,
    HoldPosition,
    Use,
    Cast
}

public enum ECommandStatus
{
    Queued,
    Interrupted,
    Executing,
    Cancelled,
    Done
}

public class Command
{
    public ECommandType commandType;
    public ECommandStatus commandStatus;

    public Command(ECommandType commandType)
    {
        this.commandType = commandType;
    }
}

public class CommandSystem : MonoBehaviour
{
    public Queue<Command> commands;
    public Command currentCommand;

    public delegate void OnCommandQueued(GameObject owner, Command queuedCommand);
    public event OnCommandQueued onCommandQueued;

    public delegate void OnCommandExecute(GameObject owner, Command executedCommand);
    public event OnCommandExecute onCommandExecute;

    public delegate void OnCommandComplete(GameObject owner, Command completedCommand);
    public event OnCommandComplete onCommandComplete;

    private void Start()
    {
        commands = new Queue<Command>();
    }

    public void SetCurrentCommand(Command command)
    {
        currentCommand = command;
    }

    public void QueueCommand(Command command, bool cancelCurrentCommand = false)
    {
        if (cancelCurrentCommand)
        {
            commands.Clear();
        }

        command.commandStatus = ECommandStatus.Executing;
        commands.Enqueue(command);

        if (onCommandQueued != null)
            onCommandQueued(gameObject, command);
    }

    [Task]
    bool IsCurrentCommand_FollowTarget()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.FollowTarget;
    }

    [Task]
    bool IsCurrentCommand_MoveAttack()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.MoveAttack;
    }

    [Task]
    bool IsCurrentCommand_MoveToTarget()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.MoveToTarget;
    }

    [Task]
    bool IsCurrentCommand_MoveToTargetCursor()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.MoveToTargetCursor;
    }

    [Task]
    bool IsCurrentCommand_AttackTarget()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.AttackTarget;
    }

    [Task]
    bool IsCurrentCommand_Follow()
    {
        if (commands.Count == 0)
            return false;

        return commands.Peek() != null && commands.Peek().commandType == ECommandType.FollowTarget;
    }

    [Task]
    bool IsCurrentCommand_Hold()
    {
        if (commands.Count == 0)
            return false;
        return commands.Peek() != null && commands.Peek().commandType == ECommandType.HoldPosition;
    }

    [Task]
    bool IsCurrentCommand_Done()
    {
        if (commands.Count == 0)
            return false;
        return commands.Peek() != null && commands.Peek().commandStatus == ECommandStatus.Done;
    }

    [Task]
    bool SetCommandDone()
    {
        if (commands.Count == 0)
            return false;

        if (commands.Peek() != null)
        {
            commands.Peek().commandStatus = ECommandStatus.Done;
        }

        return true;
    }

    [Task]
    bool PopCommand()
    {
        if (commands.Count == 0)
            return false;
        if (commands.Peek() != null)
        {
            commands.Dequeue();
        }

        return true;
    }


}
