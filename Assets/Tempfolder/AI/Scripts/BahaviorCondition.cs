using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BT
{
    /// <summary>
    /// 조건 노드의 조건 리스트
    /// </summary>
    public enum BehaviorCondition
    {
        /// <summary>
        /// A &lt; B
        /// </summary>
        LESS,
        /// <summary>
        /// A &gt; B
        /// </summary>
        GREATER,
        /// <summary>
        /// A == B
        /// </summary>
        EQUAL,
        /// <summary>
        /// A &lt; B
        /// </summary>
        LESSOREQUAL,
        /// <summary>
        /// A &gt;= B
        /// </summary>
        GREATEROREUAQL
    }
}
