using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Kit.Extend
{
	public static class TransformExtend
    {
        #region Candy tools, relative position
        /*TransformPoint is used, as the name implies to transform a point from local space to global space. For example, if the collider of a player is offset by half their height so that the transform position is at the player's feet, to get the world space position of the collider center, it would be playerTransform.TransformPoint(collider.center) because that world-space position will change if any one of the player's position, rotation, or scale changes.
         */

        /// <summary>As same as TransformPoint</summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <see cref="http://docs.unity3d.com/412/Documentation/ScriptReference/Transform.TransformPoint.html"/>
        /// <seealso cref="http://answers.unity3d.com/questions/154176/transformtransformpoint-vs-transformdirection.html"/>
        /// <seealso cref="http://answers.unity3d.com/questions/1021968/difference-between-transformtransformvector-and-tr.html"/>
        public static Vector3 PositionLocalToWorld(this Transform transform, Vector3 position)
        {
            return transform.TransformPoint(position);
        }
        /// <summary>As same as Inverse Transform Point</summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <see cref="http://docs.unity3d.com/412/Documentation/ScriptReference/Transform.InverseTransformPoint.html"/>
        public static Vector3 PositionWorldToLocal(this Transform transform, Vector3 position)
        {
            return transform.InverseTransformPoint(position);
        }

        /*TransformDirection is used to transform a direction. For example, if there were a friendly AI that always wanted to face the same direction as the player, it would set its rotation to Quaternion.LookDirection(playerTransform.TransformDirection(Vector3.forward)). There's actually a convenience property on transform called .forward that does exactly this, but bear with me. This rotates Vector3.forward (which is 0,0,1) to face the direction of the player's forward direction. So if the player's rotation is in the default orientation, it will just return 0,0,1. But if the player were looking straight down, it would return 0,-1,0. Since TransformDirection only cares about the rotation, note that the magnitude is preserved.
         */
        /// <summary>As same as TransformDirection</summary>
        /// <param name="transform"></param>
        /// <param name="localDirection"></param>
        /// <returns></returns>
        /// <see cref="http://docs.unity3d.com/412/Documentation/ScriptReference/Transform.TransformDirection.html"/>
        public static Vector3 DirectionLocalToWorld(this Transform transform, Vector3 localDirection)
        {
            return transform.TransformDirection(localDirection);
        }
        /// <summary>As same as Inverse Transform Direction</summary>
        /// <param name="transform"></param>
        /// <param name="worldDirection"></param>
        /// <returns></returns>
        /// <see cref="http://docs.unity3d.com/412/Documentation/ScriptReference/Transform.InverseTransformDirection.html"/>
        public static Vector3 DirectionWorldToLocal(this Transform transform, Vector3 worldDirection)
        {
            return transform.InverseTransformDirection(worldDirection);
        }
        
        /*TransformVector, which seems to be the same as TransformDirection, but takes scale into account and will thus change the return value's magnitude accordingly (and probably the direction too if the scale is nonuniform). I'm not actually sure what this is useful for, since my use cases always fall under the first two, but clearly you've found a use for it!
         */
        public static Vector3 DirectionDistanceLocalToWorld(this Transform transform, Vector3 vector)
        {
            return transform.TransformVector(vector);
        }
        public static Vector3 DirectionDistanceWorldToLocal(this Transform transform, Vector3 vector)
        {
            return transform.InverseTransformVector(vector);
        }

        #endregion

        #region Destroy
        /// <summary>Destroies all gameobject in children.</summary>
		/// <param name="gameobject">Gameobject.</param>
		/// <param name="immediate">If set to <c>true</c> destroy immediate.</param>
		public static void DestroyOnlyChildrens(this GameObject gameobject, bool immediate)
		{
			gameobject.transform.DestroyOnlyChildrens(immediate);
		}
		/// <summary>Destroies all gameobject in children.</summary>
		/// <param name="transform">_transform.</param>
		/// <param name="immediate">If set to <c>true</c> destroy immediate.</param>
		public static void DestroyOnlyChildrens(this Transform transform, bool immediate)
		{
			List<GameObject> children=new List<GameObject>();
			foreach(Transform child in transform)
			{
				children.Add(child.gameObject);
			}
			children.ForEach(delegate(GameObject obj)
			{
				if( immediate ) MonoBehaviour.DestroyImmediate(obj);
				else MonoBehaviour.Destroy(obj);
			});
		}
        #endregion

        #region SendMessage
        /// <summary>Sends the message in childrens.</summary>
		/// <param name="transform">_transform.</param>
		/// <param name="message">_message.</param>
		/// <param name="obj">_object.</param>
		/// <param name="sendMessageOptions">_send message options.</param>
		public static void SendMessageInChildrens(this Transform transform,
		                                          string message,
		                                          object obj=null,
		                                          SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver)
		{
			if( obj==null )
				transform.SendMessage(message,sendMessageOptions);
			else
				transform.SendMessage(message,obj,sendMessageOptions);
			foreach(Transform child in transform)
				child.SendMessageInChildrens(message,obj, sendMessageOptions);
		}
		/// <summary>Sends the message in childrens.</summary>
		/// <param name="gameobject">_gameobject.</param>
		/// <param name="message">_message.</param>
		/// <param name="obj">_object.</param>
		/// <param name="sendMessageOptions">_send message options.</param>
		public static void SendMessageInChildrens(this GameObject gameobject,
		                                          string message,
		                                          object obj=null,
		                                          SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver)
		{
			gameobject.transform.SendMessageInChildrens(message,obj,sendMessageOptions);
		}
        #endregion

        #region Get Or Add
        /// <summary>Gets or add a component.</summary>
        /// <example><code>BoxCollider boxCollider = transform.GetOrAddComponent/<BoxCollider/>();</code></example>
		/// <seealso cref="http://wiki.unity3d.com/index.php/Singleton"/>
        /// <seealso cref="http://wiki.unity3d.com/index.php/GetOrAddComponent"/>
		public static T GetOrAddComponent<T> (this Component component) where T: Component
		{
			T rst = component.GetComponent<T>();
			if( rst == null )
				rst = component.gameObject.AddComponent<T>();
			return rst;
		}

        public static T GetOrAddComponent<T> (this GameObject gameobject) where T: Component
        {
            return gameobject.transform.GetOrAddComponent<T>();
        }
        #endregion

        #region Interface
        public static bool HasInterface<T>(this Component obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }
        public static IEnumerable<T> GetInterfacesInChildren<T>(this Component obj, bool includeInactive = false) where T : class
        {
            return obj
                .GetComponentsInChildren<Component>(includeInactive)
                .OfType<T>();
        }
        #endregion
    }
}
