// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NetBpm.Test.BaseService.Model
{
	using System;
	using System.Collections;

	public class Blog
	{
		private int id;
		private string name;

        public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

        public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}