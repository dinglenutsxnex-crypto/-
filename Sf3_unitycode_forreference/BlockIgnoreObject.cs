using UnityEngine;
using UnityEngine.EventSystems;

public class BlockIgnoreObject
{
	private GameObject go;

	private bool pointerIn;

	private bool beginDrag;

	private bool isPressed;

	public bool Empty
	{
		get
		{
			return go == null;
		}
	}

	public BlockIgnoreObject(GameObject ignoredObject)
	{
		go = ignoredObject;
		pointerIn = false;
		beginDrag = false;
	}

	public bool Contains(Vector2 point)
	{
		if (!go)
		{
			return false;
		}
		return NekkiRectTransformUtility.RectTransformToScreenSpace(go.GetComponent<RectTransform>()).Contains(point);
	}

	public void CheckPointerEnterOrExit(Vector2 point)
	{
		bool flag = Contains(point);
		if (pointerIn != flag)
		{
			pointerIn = flag;
			if (pointerIn)
			{
				ExecuteEvents.Execute(go, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
			}
			else
			{
				ExecuteEvents.Execute(go, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			}
		}
	}

	public void Execute<T>(PointerEventData data, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
	{
		if (go == null)
		{
			return;
		}
		if (functor is ExecuteEvents.EventFunction<IDragHandler> && beginDrag)
		{
			ExecuteEvents.Execute(go, data, functor);
		}
		else if (functor is ExecuteEvents.EventFunction<IEndDragHandler>)
		{
			if (beginDrag)
			{
				beginDrag = false;
				ExecuteEvents.Execute(go, data, functor);
			}
		}
		else
		{
			if (!Contains(data.position))
			{
				return;
			}
			beginDrag = beginDrag || functor is ExecuteEvents.EventFunction<IBeginDragHandler>;
			isPressed = isPressed || functor is ExecuteEvents.EventFunction<IPointerDownHandler>;
			if (functor is ExecuteEvents.EventFunction<IPointerUpHandler> || functor is ExecuteEvents.EventFunction<IDropHandler>)
			{
				if (!isPressed)
				{
					return;
				}
				isPressed = false;
			}
			ExecuteEvents.Execute(go, data, functor);
		}
	}
}
