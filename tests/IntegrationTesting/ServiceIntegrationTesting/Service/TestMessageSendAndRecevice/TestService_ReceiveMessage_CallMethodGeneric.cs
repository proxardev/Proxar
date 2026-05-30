/*
 * Copyright 2026 Proxar
 * SPDX-License-Identifier: Apache-2.0
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using Proxar.ServiceCore;

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod(150)]
    [ServiceMethodExport]
    private int Call1Arg(int args1) => args1 * 2;

    [ServiceMethod(151)]
    [ServiceMethodExport]
    private int Call2Args(int args1, int args2) => (args1 + args2) * 2;

    [ServiceMethod(152)]
    [ServiceMethodExport]
    private int Call3Args(int args1, int args2, int args3) => args1 + args2 + args3 + 10;

    [ServiceMethod(153)]
    [ServiceMethodExport]
    private int Call4Args(int args1, int args2, int args3, int args4) => (args1 * args2) + (args3 * args4);

    [ServiceMethod(154)]
    [ServiceMethodExport]
    private int Call5Args(int args1, int args2, int args3, int args4, int args5) => (args1 + args2 + args3 + args4 + args5) * 2;

    [ServiceMethod(155)]
    [ServiceMethodExport]
    private int Call6Args(int args1, int args2, int args3, int args4, int args5, int args6) => args1 + args2 + args3 + args4 + args5 + args6 + 100;

    [ServiceMethod(156)]
    [ServiceMethodExport]
    private int Call7Args(int args1, int args2, int args3, int args4, int args5, int args6, int args7) => (args1 + args2 + args3) * 2 + args4 + args5 + args6 + args7;

    [ServiceMethod(157)]
    [ServiceMethodExport]
    private int Call8Args(int args1, int args2, int args3, int args4, int args5, int args6, int args7, int args8) => (args1 + args2 + args3 + args4 + args5 + args6 + args7 + args8) / 2;

    [ServiceMethod(158)]
    [ServiceMethodExport]
    private int Call9Args(int args1, int args2, int args3, int args4, int args5, int args6, int args7, int args8, int args9) => args5 * 10 + args1 + args2 + args3 + args4 + args6 + args7 + args8 + args9;
}