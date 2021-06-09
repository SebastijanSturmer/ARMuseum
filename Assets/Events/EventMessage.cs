using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Base class for event messages.
/// </summary>
public class EventMessage
{    
}

/// <summary>
/// Event message that holds a bool value;
/// </summary>
public class BoolMessage : EventMessage
{
    public bool BoolValue;

    public BoolMessage(bool value)
    {
        BoolValue = value;
    }
}

/// <summary>
/// Event message that holds an integer value.
/// </summary>
public class IntMessage : EventMessage
{
    public int IntValue;

    public IntMessage(int value)
    {
        IntValue = value;
    }
}

/// <summary>
/// Event message that holds a float value.
/// </summary>
public class FloatMessage : EventMessage
{
    public float FloatValue;

    public FloatMessage(float value)
    {
        FloatValue = value;
    }
}

/// <summary>
/// Event message that holds a string value;
/// </summary>
public class StringMessage : EventMessage
{
    public string StringValue;

    public StringMessage(string value)
    {
        StringValue = value;
    }
}

/// <summary>
/// Event message that holds a color value;
/// </summary>
public class ColorMessage : EventMessage
{
    public Color ColorValue;

    public ColorMessage(Color value)
    {
        ColorValue = value;
    }
}

/// <summary>
/// Event message that holds a Vector3 value.
/// </summary>
public class Vector2Message : EventMessage
{
    public Vector2 Vector2Value;

    public Vector2Message(Vector2 vector2)
    {
        Vector2Value = vector2;
    }
}

/// <summary>
/// Event message that holds a Vector3 value.
/// </summary>
public class Vector3Message : EventMessage
{
    public Vector3 Vector3Value;

    public Vector3Message(Vector3 vector3)
    {
        Vector3Value = vector3;
    }
}

/// <summary>
/// Event message that holds a Transform value.
/// </summary>
public class TransformMessage : EventMessage
{
    public Transform TransformValue;

    public TransformMessage(Transform transform)
    {
        TransformValue = transform;
    }
}

/// <summary>
/// Event message that holds Vector3 values needed for BezierCurve creation.
/// </summary>
public class BezierCurveMessage : EventMessage
{
    public Vector3 Point1;
    public Vector3 Point2;
    public Vector3 Point3;
    public Vector3 Point4;

    public BezierCurveMessage(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        Point4 = point4;
    }
}

/// <summary>
/// Event message that holds a GameObject value.
/// </summary>
public class GameObjectMessage : EventMessage
{
    public GameObject GameObject;

    public GameObjectMessage(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}

/// <summary>
/// Event message that holds info about pointer that initialized that event.
/// </summary>
public class PointerEventDataMessage : EventMessage
{
    public PointerEventData PointerEventData;

    public PointerEventDataMessage(PointerEventData pointerEventData)
    {
        PointerEventData = pointerEventData;
    }
}


/// <summary>
/// Event message that holds language enum.
/// </summary>
public class LanguageMessage : EventMessage
{
    public Enums.Language Language;

    public LanguageMessage(Enums.Language language)
    {
        Language = language;
    }
}

/// <summary>
/// Event message that holds question data.
/// </summary>
public class QuizQuestionMessage : EventMessage
{
    public QuizQuestion Question;

    public QuizQuestionMessage(QuizQuestion question)
    {
        Question = question;
    }
}

/// <summary>
/// Event message that holds question stats data.
/// </summary>
public class QuizStatsMessage : EventMessage
{
    public int NumberOfQuestions;
    public int NumberOfCorrectAnswers;

    public QuizStatsMessage(int numberOfQuestions, int numberOfCorrectAnswers)
    {
        NumberOfQuestions = numberOfQuestions;
        NumberOfCorrectAnswers = numberOfCorrectAnswers;
    }
}

