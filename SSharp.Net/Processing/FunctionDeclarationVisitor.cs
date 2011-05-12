﻿/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;
using System.Collections.Generic;

namespace Scripting.SSharp.Processing
{
  internal class FunctionDeclarationVisitor : IPostProcessing
  {
    private Script _script;

    #region IAstVisitor Members
    public Stack<string> _names = new Stack<string>();

    public void BeginVisit(AstNode node)
    {
      var namespaceNode = node as ScriptNamespaceDefinition;
      if (namespaceNode != null)
      {
        _names.Push(namespaceNode.Name);
      }

      var definition = node as ScriptFunctionDefinition;
      if (definition == null || string.IsNullOrEmpty(definition.Name)) return;

      definition._owner = _script;

      if (_names.Count > 0)
      {
        _script.Context.SetItem(
          string.Format(NamespaceScope.NameFormat, _names.Peek(), definition.Name), 
          definition);
        definition.namespaceName = _names.Peek();
      }
      else
      {
        _script.Context.SetItem(definition.Name, definition);
      }

      EventBroker.RegisterFunction(definition, _script);
    }

    public void EndVisit(AstNode node)
    {
      var namespaceNode = node as ScriptNamespaceDefinition;
      if (namespaceNode != null)
      {
        _names.Pop();
      }
    }

    #endregion

    #region IPostProcessing Members

    public void BeginProcessing(Script script)
    {
      _script = script;
    }

    public void EndProcessing(Script script)
    {
      if (_script != script) throw new InvalidOperationException();
      _script = null;
    }

    #endregion
  }
}
