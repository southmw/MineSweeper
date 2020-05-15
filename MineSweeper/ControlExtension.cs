using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public static class ControlExtension
    {
        /// <summary>
        /// T Type의 Delegate  선언
        /// </summary>
        /// <typeparam name="T">T타입</typeparam>
        /// <param name="obj">Invoke 할 컨트롤 </param>
        public delegate void RunOnUIThreadDelegate<T>(T obj) where T : ISynchronizeInvoke;

        /// <summary>
        /// ISynchoronizeInvoke 인터페이스 구현체 대한 RunOnUIThread확장 메소드
        /// </summary>
        /// <typeparam name="T">T타입, 캐스팅을 피하기 위해 사용</typeparam>
        /// <param name="obj">메소드 확장할 컨트롤</param>
        /// <param name="action">수행할 Action</param>
        public static void RunOnUIThread<T>(this T obj, RunOnUIThreadDelegate<T> action)
            where T : ISynchronizeInvoke
        {
            if (obj.InvokeRequired) {
                obj.Invoke(action, new object[] { obj });
            } else {
                action(obj);
            }
        }
    }
}
