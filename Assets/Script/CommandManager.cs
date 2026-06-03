using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CommandType { MoveForward, TurnLeft, TurnRight, Attack }

public class CommandManager : MonoBehaviour
{
    public List<CommandType> commandList = new List<CommandType>();
    public TextMeshProUGUI commandDisplay;
    public PlayerController player; // Tarik objek PlayerModel ke sini

    // Fungsi-fungsi ini akan dipanggil oleh Tombol UI
    public void AddMoveForward() { AddCommand(CommandType.MoveForward, "moveForward()"); }
    public void AddTurnLeft() { AddCommand(CommandType.TurnLeft, "turnLeft()"); }
    public void AddTurnRight() { AddCommand(CommandType.TurnRight, "turnRight()"); }
    public void AddAttack() { AddCommand(CommandType.Attack, "attack()"); }

    private void AddCommand(CommandType type, string text)
    {
        commandList.Add(type);
        if (commandDisplay != null) 
            commandDisplay.text += "\n" + text; // Nulis ke layar
    }

    // Dipanggil oleh tombol "Run"
    public void RunGame()
    {
        StartCoroutine(ExecuteSequence());
    }

    private IEnumerator ExecuteSequence()
    {
        // Jalankan perintah satu per satu
        foreach (CommandType cmd in commandList)
        {
            switch (cmd)
            {
                case CommandType.MoveForward: yield return StartCoroutine(player.MoveForward()); break;
                case CommandType.TurnLeft: yield return StartCoroutine(player.TurnLeft()); break;
                case CommandType.TurnRight: yield return StartCoroutine(player.TurnRight()); break;
                case CommandType.Attack: yield return StartCoroutine(player.Attack()); break;
            }
            yield return new WaitForSeconds(0.2f); // Jeda tipis antar perintah
        }

        // Hapus daftar setelah selesai jalan
        commandList.Clear();
        if (commandDisplay != null) commandDisplay.text = "Command List:";
    }
}