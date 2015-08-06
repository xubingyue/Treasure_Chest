/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.using System;

using System.Collections;
using System.Collections.Generic;

namespace Soomla.Levelup
{
	/// <summary>
	/// A specific type of <c>GatesList</c> that can be opened only if <b>ALL</b> 
	/// <c>Gate</c>s in its list are open.
	/// </summary>
	public class GatesListAND : GatesList
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="id">ID.</param>
		public GatesListAND(string id)
			: base(id)
		{
			Gates = new List<Gate>();
		}

		/// <summary>
		/// Constructor for <c>GatesList</c> with one <c>Gate</c>.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="singleGate">Single <c>Gate</c> in this <c>GatesList</c>.</param>
		public GatesListAND(string id, Gate singleGate)
			: base(id, singleGate)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="gates">List of <c>Gate</c>s.</param>
		public GatesListAND(string id, List<Gate> gates)
			: base(id, gates)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="jsonGate">JSON gate.</param>
		public GatesListAND(JSONObject jsonGate)
			: base(jsonGate)
		{
		}

		/// <summary>
		/// Checks if this <c>GatesList</c> meets its criteria for opening, by checking that ALL 
		/// <c>Gate</c>s in the list are open. 
		/// </summary>
		/// <returns>If ALL <c>Gate</c>s in this <c>GatesList</c> are open returns <c>true</c>; 
		/// otherwise <c>false</c>.</returns>
		protected override bool canOpenInner() {
			foreach (Gate gate in Gates) {
				if (!gate.IsOpen()) {
					return false;
				}
			}
			return true;
		}

	}
}

