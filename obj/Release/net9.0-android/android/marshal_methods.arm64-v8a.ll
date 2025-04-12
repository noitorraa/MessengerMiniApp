; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [152 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [456 x i64] [
	i64 u0x0071cf2d27b7d61e, ; 0: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 81
	i64 u0x018d2cc5e2de2b95, ; 1: lib_Microsoft.AspNetCore.SignalR.Common.dll.so => 40
	i64 u0x02123411c4e01926, ; 2: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 77
	i64 u0x02abedc11addc1ed, ; 3: lib_Mono.Android.Runtime.dll.so => 150
	i64 u0x032267b2a94db371, ; 4: lib_Xamarin.AndroidX.AppCompat.dll.so => 60
	i64 u0x043032f1d071fae0, ; 5: ru/Microsoft.Maui.Controls.resources => 24
	i64 u0x044440a55165631e, ; 6: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 2
	i64 u0x046eb1581a80c6b0, ; 7: vi/Microsoft.Maui.Controls.resources => 30
	i64 u0x0517ef04e06e9f76, ; 8: System.Net.Primitives => 114
	i64 u0x0565d18c6da3de38, ; 9: Xamarin.AndroidX.RecyclerView => 79
	i64 u0x0581db89237110e9, ; 10: lib_System.Collections.dll.so => 93
	i64 u0x05989cb940b225a9, ; 11: Microsoft.Maui.dll => 53
	i64 u0x06076b5d2b581f08, ; 12: zh-HK/Microsoft.Maui.Controls.resources => 31
	i64 u0x06388ffe9f6c161a, ; 13: System.Xml.Linq.dll => 144
	i64 u0x0680a433c781bb3d, ; 14: Xamarin.AndroidX.Collection.Jvm => 63
	i64 u0x07c57877c7ba78ad, ; 15: ru/Microsoft.Maui.Controls.resources.dll => 24
	i64 u0x07dcdc7460a0c5e4, ; 16: System.Collections.NonGeneric => 91
	i64 u0x08a7c865576bbde7, ; 17: System.Reflection.Primitives => 128
	i64 u0x08f3c9788ee2153c, ; 18: Xamarin.AndroidX.DrawerLayout => 68
	i64 u0x0919c28b89381a0b, ; 19: lib_Microsoft.Extensions.Options.dll.so => 49
	i64 u0x092266563089ae3e, ; 20: lib_System.Collections.NonGeneric.dll.so => 91
	i64 u0x09d144a7e214d457, ; 21: System.Security.Cryptography => 135
	i64 u0x0abb3e2b271edc45, ; 22: System.Threading.Channels.dll => 140
	i64 u0x0b3b632c3bbee20c, ; 23: sk/Microsoft.Maui.Controls.resources => 25
	i64 u0x0b6aff547b84fbe9, ; 24: Xamarin.KotlinX.Serialization.Core.Jvm => 87
	i64 u0x0b74b547d9e0e85d, ; 25: Microsoft.AspNetCore.SignalR.Protocols.Json.dll => 41
	i64 u0x0be2e1f8ce4064ed, ; 26: Xamarin.AndroidX.ViewPager => 82
	i64 u0x0c3ca6cc978e2aae, ; 27: pt-BR/Microsoft.Maui.Controls.resources => 21
	i64 u0x0c59ad9fbbd43abe, ; 28: Mono.Android => 151
	i64 u0x0c7790f60165fc06, ; 29: lib_Microsoft.Maui.Essentials.dll.so => 54
	i64 u0x0e14e73a54dda68e, ; 30: lib_System.Net.NameResolution.dll.so => 112
	i64 u0x0f85ec9030d52395, ; 31: MessengerMiniApp => 88
	i64 u0x102a31b45304b1da, ; 32: Xamarin.AndroidX.CustomView => 67
	i64 u0x10f6cfcbcf801616, ; 33: System.IO.Compression.Brotli => 104
	i64 u0x11a70d0e1009fb11, ; 34: System.Net.WebSockets.dll => 120
	i64 u0x123639456fb056da, ; 35: System.Reflection.Emit.Lightweight.dll => 127
	i64 u0x125b7f94acb989db, ; 36: Xamarin.AndroidX.RecyclerView.dll => 79
	i64 u0x13a01de0cbc3f06c, ; 37: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 8
	i64 u0x13f1e5e209e91af4, ; 38: lib_Java.Interop.dll.so => 149
	i64 u0x13f1e880c25d96d1, ; 39: he/Microsoft.Maui.Controls.resources => 9
	i64 u0x143d8ea60a6a4011, ; 40: Microsoft.Extensions.DependencyInjection.Abstractions => 45
	i64 u0x1497051b917530bd, ; 41: lib_System.Net.WebSockets.dll.so => 120
	i64 u0x15089560460fb845, ; 42: Microsoft.AspNetCore.SignalR.Client.Core => 39
	i64 u0x17b56e25558a5d36, ; 43: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 12
	i64 u0x17f9358913beb16a, ; 44: System.Text.Encodings.Web => 137
	i64 u0x18402a709e357f3b, ; 45: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 87
	i64 u0x18f0ce884e87d89a, ; 46: nb/Microsoft.Maui.Controls.resources.dll => 18
	i64 u0x1a91866a319e9259, ; 47: lib_System.Collections.Concurrent.dll.so => 90
	i64 u0x1aac34d1917ba5d3, ; 48: lib_System.dll.so => 147
	i64 u0x1aad60783ffa3e5b, ; 49: lib-th-Microsoft.Maui.Controls.resources.dll.so => 27
	i64 u0x1c753b5ff15bce1b, ; 50: Mono.Android.Runtime.dll => 150
	i64 u0x1e3d87657e9659bc, ; 51: Xamarin.AndroidX.Navigation.UI => 78
	i64 u0x1e71143913d56c10, ; 52: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 16
	i64 u0x1ed8fcce5e9b50a0, ; 53: Microsoft.Extensions.Options.dll => 49
	i64 u0x209375905fcc1bad, ; 54: lib_System.IO.Compression.Brotli.dll.so => 104
	i64 u0x2174319c0d835bc9, ; 55: System.Runtime => 134
	i64 u0x21cc7e445dcd5469, ; 56: System.Reflection.Emit.ILGeneration => 126
	i64 u0x220fd4f2e7c48170, ; 57: th/Microsoft.Maui.Controls.resources => 27
	i64 u0x237be844f1f812c7, ; 58: System.Threading.Thread.dll => 141
	i64 u0x2407aef2bbe8fadf, ; 59: System.Console => 97
	i64 u0x240abe014b27e7d3, ; 60: Xamarin.AndroidX.Core.dll => 65
	i64 u0x247619fe4413f8bf, ; 61: System.Runtime.Serialization.Primitives.dll => 133
	i64 u0x252073cc3caa62c2, ; 62: fr/Microsoft.Maui.Controls.resources.dll => 8
	i64 u0x2662c629b96b0b30, ; 63: lib_Xamarin.Kotlin.StdLib.dll.so => 85
	i64 u0x268c1439f13bcc29, ; 64: lib_Microsoft.Extensions.Primitives.dll.so => 50
	i64 u0x273f3515de5faf0d, ; 65: id/Microsoft.Maui.Controls.resources.dll => 13
	i64 u0x2742545f9094896d, ; 66: hr/Microsoft.Maui.Controls.resources => 11
	i64 u0x2759af78ab94d39b, ; 67: System.Net.WebSockets => 120
	i64 u0x27b410442fad6cf1, ; 68: Java.Interop.dll => 149
	i64 u0x2801845a2c71fbfb, ; 69: System.Net.Primitives.dll => 114
	i64 u0x2a128783efe70ba0, ; 70: uk/Microsoft.Maui.Controls.resources.dll => 29
	i64 u0x2a3b095612184159, ; 71: lib_System.Net.NetworkInformation.dll.so => 113
	i64 u0x2a6507a5ffabdf28, ; 72: System.Diagnostics.TraceSource.dll => 100
	i64 u0x2ad156c8e1354139, ; 73: fi/Microsoft.Maui.Controls.resources => 7
	i64 u0x2af298f63581d886, ; 74: System.Text.RegularExpressions.dll => 139
	i64 u0x2afc1c4f898552ee, ; 75: lib_System.Formats.Asn1.dll.so => 103
	i64 u0x2b148910ed40fbf9, ; 76: zh-Hant/Microsoft.Maui.Controls.resources.dll => 33
	i64 u0x2c8bd14bb93a7d82, ; 77: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 20
	i64 u0x2cc9e1fed6257257, ; 78: lib_System.Reflection.Emit.Lightweight.dll.so => 127
	i64 u0x2cd723e9fe623c7c, ; 79: lib_System.Private.Xml.Linq.dll.so => 124
	i64 u0x2d169d318a968379, ; 80: System.Threading.dll => 142
	i64 u0x2d47774b7d993f59, ; 81: sv/Microsoft.Maui.Controls.resources.dll => 26
	i64 u0x2db915caf23548d2, ; 82: System.Text.Json.dll => 138
	i64 u0x2e6f1f226821322a, ; 83: el/Microsoft.Maui.Controls.resources.dll => 5
	i64 u0x2e7c9658c7fb7927, ; 84: Microsoft.Extensions.Features.dll => 46
	i64 u0x2f2e98e1c89b1aff, ; 85: System.Xml.ReaderWriter => 145
	i64 u0x309ee9eeec09a71e, ; 86: lib_Xamarin.AndroidX.Fragment.dll.so => 69
	i64 u0x31195fef5d8fb552, ; 87: _Microsoft.Android.Resource.Designer.dll => 34
	i64 u0x32243413e774362a, ; 88: Xamarin.AndroidX.CardView.dll => 62
	i64 u0x3235427f8d12dae1, ; 89: lib_System.Drawing.Primitives.dll.so => 101
	i64 u0x329753a17a517811, ; 90: fr/Microsoft.Maui.Controls.resources => 8
	i64 u0x32aa989ff07a84ff, ; 91: lib_System.Xml.ReaderWriter.dll.so => 145
	i64 u0x33a31443733849fe, ; 92: lib-es-Microsoft.Maui.Controls.resources.dll.so => 6
	i64 u0x341abc357fbb4ebf, ; 93: lib_System.Net.Sockets.dll.so => 117
	i64 u0x34dfd74fe2afcf37, ; 94: Microsoft.Maui => 53
	i64 u0x34e292762d9615df, ; 95: cs/Microsoft.Maui.Controls.resources.dll => 2
	i64 u0x3508234247f48404, ; 96: Microsoft.Maui.Controls => 51
	i64 u0x3549870798b4cd30, ; 97: lib_Xamarin.AndroidX.ViewPager2.dll.so => 83
	i64 u0x355282fc1c909694, ; 98: Microsoft.Extensions.Configuration => 42
	i64 u0x36b2b50fdf589ae2, ; 99: System.Reflection.Emit.Lightweight => 127
	i64 u0x374ef46b06791af6, ; 100: System.Reflection.Primitives.dll => 128
	i64 u0x385c17636bb6fe6e, ; 101: Xamarin.AndroidX.CustomView.dll => 67
	i64 u0x38869c811d74050e, ; 102: System.Net.NameResolution.dll => 112
	i64 u0x393c226616977fdb, ; 103: lib_Xamarin.AndroidX.ViewPager.dll.so => 82
	i64 u0x395e37c3334cf82a, ; 104: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 1
	i64 u0x39aa39fda111d9d3, ; 105: Newtonsoft.Json => 56
	i64 u0x3b860f9932505633, ; 106: lib_System.Text.Encoding.Extensions.dll.so => 136
	i64 u0x3c7c495f58ac5ee9, ; 107: Xamarin.Kotlin.StdLib => 85
	i64 u0x3d46f0b995082740, ; 108: System.Xml.Linq => 144
	i64 u0x3d9c2a242b040a50, ; 109: lib_Xamarin.AndroidX.Core.dll.so => 65
	i64 u0x407a10bb4bf95829, ; 110: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 75
	i64 u0x41cab042be111c34, ; 111: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 61
	i64 u0x434c4e1d9284cdae, ; 112: Mono.Android.dll => 151
	i64 u0x43950f84de7cc79a, ; 113: pl/Microsoft.Maui.Controls.resources.dll => 20
	i64 u0x448bd33429269b19, ; 114: Microsoft.CSharp => 89
	i64 u0x4499fa3c8e494654, ; 115: lib_System.Runtime.Serialization.Primitives.dll.so => 133
	i64 u0x4515080865a951a5, ; 116: Xamarin.Kotlin.StdLib.dll => 85
	i64 u0x45c40276a42e283e, ; 117: System.Diagnostics.TraceSource => 100
	i64 u0x46a4213bc97fe5ae, ; 118: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 24
	i64 u0x47358bd471172e1d, ; 119: lib_System.Xml.Linq.dll.so => 144
	i64 u0x47daf4e1afbada10, ; 120: pt/Microsoft.Maui.Controls.resources => 22
	i64 u0x48a6d2fa2eb5d049, ; 121: Microsoft.AspNetCore.SignalR.Protocols.Json => 41
	i64 u0x49e952f19a4e2022, ; 122: System.ObjectModel => 122
	i64 u0x4a5667b2462a664b, ; 123: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 78
	i64 u0x4b7b6532ded934b7, ; 124: System.Text.Json => 138
	i64 u0x4c7755cf07ad2d5f, ; 125: System.Net.Http.Json.dll => 110
	i64 u0x4cc5f15266470798, ; 126: lib_Xamarin.AndroidX.Loader.dll.so => 74
	i64 u0x4cf6f67dc77aacd2, ; 127: System.Net.NetworkInformation.dll => 113
	i64 u0x4d3183dd245425d4, ; 128: System.Net.WebSockets.Client.dll => 119
	i64 u0x4d479f968a05e504, ; 129: System.Linq.Expressions.dll => 107
	i64 u0x4d55a010ffc4faff, ; 130: System.Private.Xml => 125
	i64 u0x4d95fccc1f67c7ca, ; 131: System.Runtime.Loader.dll => 130
	i64 u0x4dcf44c3c9b076a2, ; 132: it/Microsoft.Maui.Controls.resources.dll => 14
	i64 u0x4dd9247f1d2c3235, ; 133: Xamarin.AndroidX.Loader.dll => 74
	i64 u0x4e32f00cb0937401, ; 134: Mono.Android.Runtime => 150
	i64 u0x4e39d45ce072e04b, ; 135: Microsoft.AspNetCore.SignalR.Common.dll => 40
	i64 u0x4ebd0c4b82c5eefc, ; 136: lib_System.Threading.Channels.dll.so => 140
	i64 u0x4f21ee6ef9eb527e, ; 137: ca/Microsoft.Maui.Controls.resources => 1
	i64 u0x5037f0be3c28c7a3, ; 138: lib_Microsoft.Maui.Controls.dll.so => 51
	i64 u0x5131bbe80989093f, ; 139: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 72
	i64 u0x51bb8a2afe774e32, ; 140: System.Drawing => 102
	i64 u0x526ce79eb8e90527, ; 141: lib_System.Net.Primitives.dll.so => 114
	i64 u0x52829f00b4467c38, ; 142: lib_System.Data.Common.dll.so => 98
	i64 u0x529ffe06f39ab8db, ; 143: Xamarin.AndroidX.Core => 65
	i64 u0x52ff996554dbf352, ; 144: Microsoft.Maui.Graphics => 55
	i64 u0x535f7e40e8fef8af, ; 145: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 25
	i64 u0x53a96d5c86c9e194, ; 146: System.Net.NetworkInformation => 113
	i64 u0x53c3014b9437e684, ; 147: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 31
	i64 u0x54795225dd1587af, ; 148: lib_System.Runtime.dll.so => 134
	i64 u0x556e8b63b660ab8b, ; 149: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 70
	i64 u0x5588627c9a108ec9, ; 150: System.Collections.Specialized => 92
	i64 u0x571c5cfbec5ae8e2, ; 151: System.Private.Uri => 123
	i64 u0x579a06fed6eec900, ; 152: System.Private.CoreLib.dll => 148
	i64 u0x57c542c14049b66d, ; 153: System.Diagnostics.DiagnosticSource => 99
	i64 u0x58601b2dda4a27b9, ; 154: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 15
	i64 u0x58688d9af496b168, ; 155: Microsoft.Extensions.DependencyInjection.dll => 44
	i64 u0x595a356d23e8da9a, ; 156: lib_Microsoft.CSharp.dll.so => 89
	i64 u0x5a89a886ae30258d, ; 157: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 64
	i64 u0x5a8f6699f4a1caa9, ; 158: lib_System.Threading.dll.so => 142
	i64 u0x5ae9cd33b15841bf, ; 159: System.ComponentModel => 96
	i64 u0x5b247cf480c75903, ; 160: Microsoft.AspNetCore.Http.Connections.Common.dll => 37
	i64 u0x5b5f0e240a06a2a2, ; 161: da/Microsoft.Maui.Controls.resources.dll => 3
	i64 u0x5c294d94f201783b, ; 162: lib_Microsoft.AspNetCore.Http.Connections.Client.dll.so => 36
	i64 u0x5c393624b8176517, ; 163: lib_Microsoft.Extensions.Logging.dll.so => 47
	i64 u0x5d0a4a29b02d9d3c, ; 164: System.Net.WebHeaderCollection.dll => 118
	i64 u0x5db0cbbd1028510e, ; 165: lib_System.Runtime.InteropServices.dll.so => 129
	i64 u0x5db30905d3e5013b, ; 166: Xamarin.AndroidX.Collection.Jvm.dll => 63
	i64 u0x5e467bc8f09ad026, ; 167: System.Collections.Specialized.dll => 92
	i64 u0x5ea92fdb19ec8c4c, ; 168: System.Text.Encodings.Web.dll => 137
	i64 u0x5eb8046dd40e9ac3, ; 169: System.ComponentModel.Primitives => 94
	i64 u0x5f36ccf5c6a57e24, ; 170: System.Xml.ReaderWriter.dll => 145
	i64 u0x5f4294b9b63cb842, ; 171: System.Data.Common => 98
	i64 u0x5f9a2d823f664957, ; 172: lib-el-Microsoft.Maui.Controls.resources.dll.so => 5
	i64 u0x609f4b7b63d802d4, ; 173: lib_Microsoft.Extensions.DependencyInjection.dll.so => 44
	i64 u0x60cd4e33d7e60134, ; 174: Xamarin.KotlinX.Coroutines.Core.Jvm => 86
	i64 u0x60f62d786afcf130, ; 175: System.Memory => 109
	i64 u0x61be8d1299194243, ; 176: Microsoft.Maui.Controls.Xaml => 52
	i64 u0x61d2cba29557038f, ; 177: de/Microsoft.Maui.Controls.resources => 4
	i64 u0x61d88f399afb2f45, ; 178: lib_System.Runtime.Loader.dll.so => 130
	i64 u0x622eef6f9e59068d, ; 179: System.Private.CoreLib => 148
	i64 u0x6400f68068c1e9f1, ; 180: Xamarin.Google.Android.Material.dll => 84
	i64 u0x64b61dd9da8a4d57, ; 181: System.Net.ServerSentEvents.dll => 58
	i64 u0x65ecac39144dd3cc, ; 182: Microsoft.Maui.Controls.dll => 51
	i64 u0x65ece51227bfa724, ; 183: lib_System.Runtime.Numerics.dll.so => 131
	i64 u0x6692e924eade1b29, ; 184: lib_System.Console.dll.so => 97
	i64 u0x66a4e5c6a3fb0bae, ; 185: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 72
	i64 u0x66d13304ce1a3efa, ; 186: Xamarin.AndroidX.CursorAdapter => 66
	i64 u0x672a10d319608935, ; 187: lib_Microsoft.AspNetCore.Http.Connections.Common.dll.so => 37
	i64 u0x68558ec653afa616, ; 188: lib-da-Microsoft.Maui.Controls.resources.dll.so => 3
	i64 u0x6872ec7a2e36b1ac, ; 189: System.Drawing.Primitives.dll => 101
	i64 u0x68fbbbe2eb455198, ; 190: System.Formats.Asn1 => 103
	i64 u0x69063fc0ba8e6bdd, ; 191: he/Microsoft.Maui.Controls.resources.dll => 9
	i64 u0x6a4d7577b2317255, ; 192: System.Runtime.InteropServices.dll => 129
	i64 u0x6ace3b74b15ee4a4, ; 193: nb/Microsoft.Maui.Controls.resources => 18
	i64 u0x6d12bfaa99c72b1f, ; 194: lib_Microsoft.Maui.Graphics.dll.so => 55
	i64 u0x6d79993361e10ef2, ; 195: Microsoft.Extensions.Primitives => 50
	i64 u0x6d86d56b84c8eb71, ; 196: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 66
	i64 u0x6d9bea6b3e895cf7, ; 197: Microsoft.Extensions.Primitives.dll => 50
	i64 u0x6e25a02c3833319a, ; 198: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 76
	i64 u0x6fd2265da78b93a4, ; 199: lib_Microsoft.Maui.dll.so => 53
	i64 u0x6fdfc7de82c33008, ; 200: cs/Microsoft.Maui.Controls.resources => 2
	i64 u0x70e99f48c05cb921, ; 201: tr/Microsoft.Maui.Controls.resources.dll => 28
	i64 u0x70fd3deda22442d2, ; 202: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 18
	i64 u0x71a495ea3761dde8, ; 203: lib-it-Microsoft.Maui.Controls.resources.dll.so => 14
	i64 u0x71ad672adbe48f35, ; 204: System.ComponentModel.Primitives.dll => 94
	i64 u0x7242820f67bc4ad6, ; 205: Microsoft.AspNetCore.SignalR.Common => 40
	i64 u0x72b1fb4109e08d7b, ; 206: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 11
	i64 u0x73e4ce94e2eb6ffc, ; 207: lib_System.Memory.dll.so => 109
	i64 u0x755a91767330b3d4, ; 208: lib_Microsoft.Extensions.Configuration.dll.so => 42
	i64 u0x758463c93f0d589e, ; 209: lib_Microsoft.AspNetCore.Connections.Abstractions.dll.so => 35
	i64 u0x76012e7334db86e5, ; 210: lib_Xamarin.AndroidX.SavedState.dll.so => 80
	i64 u0x76ca07b878f44da0, ; 211: System.Runtime.Numerics.dll => 131
	i64 u0x77d9074d8f33a303, ; 212: lib_System.Net.ServerSentEvents.dll.so => 58
	i64 u0x780bc73597a503a9, ; 213: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 17
	i64 u0x783606d1e53e7a1a, ; 214: th/Microsoft.Maui.Controls.resources.dll => 27
	i64 u0x78a1938b89c96721, ; 215: Microsoft.AspNetCore.Http.Connections.Common => 37
	i64 u0x78a45e51311409b6, ; 216: Xamarin.AndroidX.Fragment.dll => 69
	i64 u0x7adb8da2ac89b647, ; 217: fi/Microsoft.Maui.Controls.resources.dll => 7
	i64 u0x7bef86a4335c4870, ; 218: System.ComponentModel.TypeConverter => 95
	i64 u0x7c0820144cd34d6a, ; 219: sk/Microsoft.Maui.Controls.resources.dll => 25
	i64 u0x7c2a0bd1e0f988fc, ; 220: lib-de-Microsoft.Maui.Controls.resources.dll.so => 4
	i64 u0x7d49c593eeb09ac9, ; 221: Microsoft.AspNetCore.SignalR.Client.dll => 38
	i64 u0x7d649b75d580bb42, ; 222: ms/Microsoft.Maui.Controls.resources.dll => 17
	i64 u0x7d8ee2bdc8e3aad1, ; 223: System.Numerics.Vectors => 121
	i64 u0x7dfc3d6d9d8d7b70, ; 224: System.Collections => 93
	i64 u0x7e946809d6008ef2, ; 225: lib_System.ObjectModel.dll.so => 122
	i64 u0x7ecc13347c8fd849, ; 226: lib_System.ComponentModel.dll.so => 96
	i64 u0x7f00ddd9b9ca5a13, ; 227: Xamarin.AndroidX.ViewPager.dll => 82
	i64 u0x7f9351cd44b1273f, ; 228: Microsoft.Extensions.Configuration.Abstractions => 43
	i64 u0x7fbd557c99b3ce6f, ; 229: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 71
	i64 u0x809b1d586a1c34b9, ; 230: Plugin.Maui.Audio => 57
	i64 u0x812c069d5cdecc17, ; 231: System.dll => 147
	i64 u0x81ab745f6c0f5ce6, ; 232: zh-Hant/Microsoft.Maui.Controls.resources => 33
	i64 u0x8277f2be6b5ce05f, ; 233: Xamarin.AndroidX.AppCompat => 60
	i64 u0x828f06563b30bc50, ; 234: lib_Xamarin.AndroidX.CardView.dll.so => 62
	i64 u0x82df8f5532a10c59, ; 235: lib_System.Drawing.dll.so => 102
	i64 u0x82f6403342e12049, ; 236: uk/Microsoft.Maui.Controls.resources => 29
	i64 u0x83c14ba66c8e2b8c, ; 237: zh-Hans/Microsoft.Maui.Controls.resources => 32
	i64 u0x846f52335a832137, ; 238: Microsoft.Extensions.Features => 46
	i64 u0x86a909228dc7657b, ; 239: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 33
	i64 u0x86b3e00c36b84509, ; 240: Microsoft.Extensions.Configuration.dll => 42
	i64 u0x87c69b87d9283884, ; 241: lib_System.Threading.Thread.dll.so => 141
	i64 u0x87f6569b25707834, ; 242: System.IO.Compression.Brotli.dll => 104
	i64 u0x8842b3a5d2d3fb36, ; 243: Microsoft.Maui.Essentials => 54
	i64 u0x88bda98e0cffb7a9, ; 244: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 86
	i64 u0x897a606c9e39c75f, ; 245: lib_System.ComponentModel.Primitives.dll.so => 94
	i64 u0x8ad229ea26432ee2, ; 246: Xamarin.AndroidX.Loader => 74
	i64 u0x8b42b55a5bb040b5, ; 247: lib_Microsoft.AspNetCore.SignalR.Protocols.Json.dll.so => 41
	i64 u0x8b4ff5d0fdd5faa1, ; 248: lib_System.Diagnostics.DiagnosticSource.dll.so => 99
	i64 u0x8b9ceca7acae3451, ; 249: lib-he-Microsoft.Maui.Controls.resources.dll.so => 9
	i64 u0x8d0f420977c2c1c7, ; 250: Xamarin.AndroidX.CursorAdapter.dll => 66
	i64 u0x8d7b8ab4b3310ead, ; 251: System.Threading => 142
	i64 u0x8da188285aadfe8e, ; 252: System.Collections.Concurrent => 90
	i64 u0x8ec6e06a61c1baeb, ; 253: lib_Newtonsoft.Json.dll.so => 56
	i64 u0x8ed807bfe9858dfc, ; 254: Xamarin.AndroidX.Navigation.Common => 75
	i64 u0x8ee08b8194a30f48, ; 255: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 10
	i64 u0x8ef7601039857a44, ; 256: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 23
	i64 u0x8f32c6f611f6ffab, ; 257: pt/Microsoft.Maui.Controls.resources.dll => 22
	i64 u0x8f8829d21c8985a4, ; 258: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 21
	i64 u0x90263f8448b8f572, ; 259: lib_System.Diagnostics.TraceSource.dll.so => 100
	i64 u0x903101b46fb73a04, ; 260: _Microsoft.Android.Resource.Designer => 34
	i64 u0x90393bd4865292f3, ; 261: lib_System.IO.Compression.dll.so => 105
	i64 u0x90634f86c5ebe2b5, ; 262: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 72
	i64 u0x907b636704ad79ef, ; 263: lib_Microsoft.Maui.Controls.Xaml.dll.so => 52
	i64 u0x90ae2b5b8b652f2a, ; 264: lib_Microsoft.AspNetCore.SignalR.Client.Core.dll.so => 39
	i64 u0x91418dc638b29e68, ; 265: lib_Xamarin.AndroidX.CustomView.dll.so => 67
	i64 u0x9157bd523cd7ed36, ; 266: lib_System.Text.Json.dll.so => 138
	i64 u0x91a74f07b30d37e2, ; 267: System.Linq.dll => 108
	i64 u0x91fa41a87223399f, ; 268: ca/Microsoft.Maui.Controls.resources.dll => 1
	i64 u0x93cfa73ab28d6e35, ; 269: ms/Microsoft.Maui.Controls.resources => 17
	i64 u0x944077d8ca3c6580, ; 270: System.IO.Compression.dll => 105
	i64 u0x966c7dca7920f48a, ; 271: MessengerMiniApp.dll => 88
	i64 u0x967fc325e09bfa8c, ; 272: es/Microsoft.Maui.Controls.resources => 6
	i64 u0x9732d8dbddea3d9a, ; 273: id/Microsoft.Maui.Controls.resources => 13
	i64 u0x978be80e5210d31b, ; 274: Microsoft.Maui.Graphics.dll => 55
	i64 u0x97b8c771ea3e4220, ; 275: System.ComponentModel.dll => 96
	i64 u0x97e144c9d3c6976e, ; 276: System.Collections.Concurrent.dll => 90
	i64 u0x991d510397f92d9d, ; 277: System.Linq.Expressions => 107
	i64 u0x99a00ca5270c6878, ; 278: Xamarin.AndroidX.Navigation.Runtime => 77
	i64 u0x99cdc6d1f2d3a72f, ; 279: ko/Microsoft.Maui.Controls.resources.dll => 16
	i64 u0x9d5dbcf5a48583fe, ; 280: lib_Xamarin.AndroidX.Activity.dll.so => 59
	i64 u0x9d74dee1a7725f34, ; 281: Microsoft.Extensions.Configuration.Abstractions.dll => 43
	i64 u0x9e4534b6adaf6e84, ; 282: nl/Microsoft.Maui.Controls.resources => 19
	i64 u0x9eaf1efdf6f7267e, ; 283: Xamarin.AndroidX.Navigation.Common.dll => 75
	i64 u0x9ef542cf1f78c506, ; 284: Xamarin.AndroidX.Lifecycle.LiveData.Core => 71
	i64 u0xa05475503f80b7d9, ; 285: Microsoft.AspNetCore.Connections.Abstractions => 35
	i64 u0xa0d8259f4cc284ec, ; 286: lib_System.Security.Cryptography.dll.so => 135
	i64 u0xa1440773ee9d341e, ; 287: Xamarin.Google.Android.Material => 84
	i64 u0xa1b9d7c27f47219f, ; 288: Xamarin.AndroidX.Navigation.UI.dll => 78
	i64 u0xa2572680829d2c7c, ; 289: System.IO.Pipelines.dll => 106
	i64 u0xa46aa1eaa214539b, ; 290: ko/Microsoft.Maui.Controls.resources => 16
	i64 u0xa4edc8f2ceae241a, ; 291: System.Data.Common.dll => 98
	i64 u0xa5494f40f128ce6a, ; 292: System.Runtime.Serialization.Formatters.dll => 132
	i64 u0xa5e599d1e0524750, ; 293: System.Numerics.Vectors.dll => 121
	i64 u0xa5f1ba49b85dd355, ; 294: System.Security.Cryptography.dll => 135
	i64 u0xa67dbee13e1df9ca, ; 295: Xamarin.AndroidX.SavedState.dll => 80
	i64 u0xa68a420042bb9b1f, ; 296: Xamarin.AndroidX.DrawerLayout.dll => 68
	i64 u0xa78ce3745383236a, ; 297: Xamarin.AndroidX.Lifecycle.Common.Jvm => 70
	i64 u0xa7c31b56b4dc7b33, ; 298: hu/Microsoft.Maui.Controls.resources => 12
	i64 u0xaa2219c8e3449ff5, ; 299: Microsoft.Extensions.Logging.Abstractions => 48
	i64 u0xaa443ac34067eeef, ; 300: System.Private.Xml.dll => 125
	i64 u0xaa52de307ef5d1dd, ; 301: System.Net.Http => 111
	i64 u0xaaaf86367285a918, ; 302: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 45
	i64 u0xaaf84bb3f052a265, ; 303: el/Microsoft.Maui.Controls.resources => 5
	i64 u0xab9c1b2687d86b0b, ; 304: lib_System.Linq.Expressions.dll.so => 107
	i64 u0xac03339b985f4d59, ; 305: Microsoft.AspNetCore.SignalR.Client.Core.dll => 39
	i64 u0xac2af3fa195a15ce, ; 306: System.Runtime.Numerics => 131
	i64 u0xac5376a2a538dc10, ; 307: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 71
	i64 u0xac98d31068e24591, ; 308: System.Xml.XDocument => 146
	i64 u0xacd46e002c3ccb97, ; 309: ro/Microsoft.Maui.Controls.resources => 23
	i64 u0xacf42eea7ef9cd12, ; 310: System.Threading.Channels => 140
	i64 u0xad89c07347f1bad6, ; 311: nl/Microsoft.Maui.Controls.resources.dll => 19
	i64 u0xadbb53caf78a79d2, ; 312: System.Web.HttpUtility => 143
	i64 u0xadc90ab061a9e6e4, ; 313: System.ComponentModel.TypeConverter.dll => 95
	i64 u0xadf511667bef3595, ; 314: System.Net.Security => 116
	i64 u0xae282bcd03739de7, ; 315: Java.Interop => 149
	i64 u0xae53579c90db1107, ; 316: System.ObjectModel.dll => 122
	i64 u0xafe29f45095518e7, ; 317: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 73
	i64 u0xb05cc42cd94c6d9d, ; 318: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 26
	i64 u0xb220631954820169, ; 319: System.Text.RegularExpressions => 139
	i64 u0xb2a3f67f3bf29fce, ; 320: da/Microsoft.Maui.Controls.resources => 3
	i64 u0xb3f0a0fcda8d3ebc, ; 321: Xamarin.AndroidX.CardView => 62
	i64 u0xb46be1aa6d4fff93, ; 322: hi/Microsoft.Maui.Controls.resources => 10
	i64 u0xb477491be13109d8, ; 323: ar/Microsoft.Maui.Controls.resources => 0
	i64 u0xb4bd7015ecee9d86, ; 324: System.IO.Pipelines => 106
	i64 u0xb5c7fcdafbc67ee4, ; 325: Microsoft.Extensions.Logging.Abstractions.dll => 48
	i64 u0xb7212c4683a94afe, ; 326: System.Drawing.Primitives => 101
	i64 u0xb7b7753d1f319409, ; 327: sv/Microsoft.Maui.Controls.resources => 26
	i64 u0xb81a2c6e0aee50fe, ; 328: lib_System.Private.CoreLib.dll.so => 148
	i64 u0xb9185c33a1643eed, ; 329: Microsoft.CSharp.dll => 89
	i64 u0xb9f64d3b230def68, ; 330: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 22
	i64 u0xb9fc3c8a556e3691, ; 331: ja/Microsoft.Maui.Controls.resources => 15
	i64 u0xba4670aa94a2b3c6, ; 332: lib_System.Xml.XDocument.dll.so => 146
	i64 u0xba48785529705af9, ; 333: System.Collections.dll => 93
	i64 u0xbb65706fde942ce3, ; 334: System.Net.Sockets => 117
	i64 u0xbbd180354b67271a, ; 335: System.Runtime.Serialization.Formatters => 132
	i64 u0xbd0e2c0d55246576, ; 336: System.Net.Http.dll => 111
	i64 u0xbd437a2cdb333d0d, ; 337: Xamarin.AndroidX.ViewPager2 => 83
	i64 u0xbee38d4a88835966, ; 338: Xamarin.AndroidX.AppCompat.AppCompatResources => 61
	i64 u0xbfc1e1fb3095f2b3, ; 339: lib_System.Net.Http.Json.dll.so => 110
	i64 u0xc040a4ab55817f58, ; 340: ar/Microsoft.Maui.Controls.resources.dll => 0
	i64 u0xc0d928351ab5ca77, ; 341: System.Console.dll => 97
	i64 u0xc12b8b3afa48329c, ; 342: lib_System.Linq.dll.so => 108
	i64 u0xc1ff9ae3cdb6e1e6, ; 343: Xamarin.AndroidX.Activity.dll => 59
	i64 u0xc28c50f32f81cc73, ; 344: ja/Microsoft.Maui.Controls.resources.dll => 15
	i64 u0xc2bcfec99f69365e, ; 345: Xamarin.AndroidX.ViewPager2.dll => 83
	i64 u0xc421b61fd853169d, ; 346: lib_System.Net.WebSockets.Client.dll.so => 119
	i64 u0xc4d3858ed4d08512, ; 347: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 73
	i64 u0xc4f2d57c50beb816, ; 348: lib_Microsoft.Extensions.Features.dll.so => 46
	i64 u0xc50fded0ded1418c, ; 349: lib_System.ComponentModel.TypeConverter.dll.so => 95
	i64 u0xc519125d6bc8fb11, ; 350: lib_System.Net.Requests.dll.so => 115
	i64 u0xc5293b19e4dc230e, ; 351: Xamarin.AndroidX.Navigation.Fragment => 76
	i64 u0xc5325b2fcb37446f, ; 352: lib_System.Private.Xml.dll.so => 125
	i64 u0xc5a0f4b95a699af7, ; 353: lib_System.Private.Uri.dll.so => 123
	i64 u0xc5de3dcae13c325f, ; 354: Microsoft.AspNetCore.SignalR.Client => 38
	i64 u0xc7c01e7d7c93a110, ; 355: System.Text.Encoding.Extensions.dll => 136
	i64 u0xc7ce851898a4548e, ; 356: lib_System.Web.HttpUtility.dll.so => 143
	i64 u0xc858a28d9ee5a6c5, ; 357: lib_System.Collections.Specialized.dll.so => 92
	i64 u0xca3a723e7342c5b6, ; 358: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 28
	i64 u0xcab3493c70141c2d, ; 359: pl/Microsoft.Maui.Controls.resources => 20
	i64 u0xcacfddc9f7c6de76, ; 360: ro/Microsoft.Maui.Controls.resources.dll => 23
	i64 u0xcb06c02bdb7bec6a, ; 361: lib_MessengerMiniApp.dll.so => 88
	i64 u0xcbd4fdd9cef4a294, ; 362: lib__Microsoft.Android.Resource.Designer.dll.so => 34
	i64 u0xcc2876b32ef2794c, ; 363: lib_System.Text.RegularExpressions.dll.so => 139
	i64 u0xcc5c3bb714c4561e, ; 364: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 86
	i64 u0xcc76886e09b88260, ; 365: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 87
	i64 u0xcce5f0b382db16b7, ; 366: Microsoft.AspNetCore.Http.Connections.Client => 36
	i64 u0xccf25c4b634ccd3a, ; 367: zh-Hans/Microsoft.Maui.Controls.resources.dll => 32
	i64 u0xcd10a42808629144, ; 368: System.Net.Requests => 115
	i64 u0xcdd0c48b6937b21c, ; 369: Xamarin.AndroidX.SwipeRefreshLayout => 81
	i64 u0xcf23d8093f3ceadf, ; 370: System.Diagnostics.DiagnosticSource.dll => 99
	i64 u0xcf8fc898f98b0d34, ; 371: System.Private.Xml.Linq => 124
	i64 u0xd1194e1d8a8de83c, ; 372: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 70
	i64 u0xd333d0af9e423810, ; 373: System.Runtime.InteropServices => 129
	i64 u0xd3426d966bb704f5, ; 374: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 61
	i64 u0xd3651b6fc3125825, ; 375: System.Private.Uri.dll => 123
	i64 u0xd373685349b1fe8b, ; 376: Microsoft.Extensions.Logging.dll => 47
	i64 u0xd3e4c8d6a2d5d470, ; 377: it/Microsoft.Maui.Controls.resources => 14
	i64 u0xd4645626dffec99d, ; 378: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 45
	i64 u0xd52f53c4b3d62e11, ; 379: Microsoft.AspNetCore.Connections.Abstractions.dll => 35
	i64 u0xd5507e11a2b2839f, ; 380: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 73
	i64 u0xd6694f8359737e4e, ; 381: Xamarin.AndroidX.SavedState => 80
	i64 u0xd6d21782156bc35b, ; 382: Xamarin.AndroidX.SwipeRefreshLayout.dll => 81
	i64 u0xd72329819cbbbc44, ; 383: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 43
	i64 u0xd7b3764ada9d341d, ; 384: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 48
	i64 u0xda1dfa4c534a9251, ; 385: Microsoft.Extensions.DependencyInjection => 44
	i64 u0xdad05a11827959a3, ; 386: System.Collections.NonGeneric.dll => 91
	i64 u0xdb5383ab5865c007, ; 387: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 30
	i64 u0xdb58816721c02a59, ; 388: lib_System.Reflection.Emit.ILGeneration.dll.so => 126
	i64 u0xdbeda89f832aa805, ; 389: vi/Microsoft.Maui.Controls.resources.dll => 30
	i64 u0xdbf9607a441b4505, ; 390: System.Linq => 108
	i64 u0xdce2c53525640bf3, ; 391: Microsoft.Extensions.Logging => 47
	i64 u0xdd2b722d78ef5f43, ; 392: System.Runtime.dll => 134
	i64 u0xdd67031857c72f96, ; 393: lib_System.Text.Encodings.Web.dll.so => 137
	i64 u0xdde30e6b77aa6f6c, ; 394: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 32
	i64 u0xde110ae80fa7c2e2, ; 395: System.Xml.XDocument.dll => 146
	i64 u0xde8769ebda7d8647, ; 396: hr/Microsoft.Maui.Controls.resources.dll => 11
	i64 u0xdf9c7682560a9629, ; 397: System.Net.ServerSentEvents => 58
	i64 u0xe0142572c095a480, ; 398: Xamarin.AndroidX.AppCompat.dll => 60
	i64 u0xe02f89350ec78051, ; 399: Xamarin.AndroidX.CoordinatorLayout.dll => 64
	i64 u0xe02ff568f8e5f275, ; 400: Microsoft.AspNetCore.Http.Connections.Client.dll => 36
	i64 u0xe192a588d4410686, ; 401: lib_System.IO.Pipelines.dll.so => 106
	i64 u0xe1a08bd3fa539e0d, ; 402: System.Runtime.Loader => 130
	i64 u0xe1b52f9f816c70ef, ; 403: System.Private.Xml.Linq.dll => 124
	i64 u0xe1ecfdb7fff86067, ; 404: System.Net.Security.dll => 116
	i64 u0xe2420585aeceb728, ; 405: System.Net.Requests.dll => 115
	i64 u0xe29b73bc11392966, ; 406: lib-id-Microsoft.Maui.Controls.resources.dll.so => 13
	i64 u0xe3811d68d4fe8463, ; 407: pt-BR/Microsoft.Maui.Controls.resources.dll => 21
	i64 u0xe494f7ced4ecd10a, ; 408: hu/Microsoft.Maui.Controls.resources.dll => 12
	i64 u0xe4a9b1e40d1e8917, ; 409: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 7
	i64 u0xe4f74a0b5bf9703f, ; 410: System.Runtime.Serialization.Primitives => 133
	i64 u0xe5434e8a119ceb69, ; 411: lib_Mono.Android.dll.so => 151
	i64 u0xe89a2a9ef110899b, ; 412: System.Drawing.dll => 102
	i64 u0xedc4817167106c23, ; 413: System.Net.Sockets.dll => 117
	i64 u0xedc632067fb20ff3, ; 414: System.Memory.dll => 109
	i64 u0xedc8e4ca71a02a8b, ; 415: Xamarin.AndroidX.Navigation.Runtime.dll => 77
	i64 u0xeeb7ebb80150501b, ; 416: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 63
	i64 u0xef72742e1bcca27a, ; 417: Microsoft.Maui.Essentials.dll => 54
	i64 u0xefec0b7fdc57ec42, ; 418: Xamarin.AndroidX.Activity => 59
	i64 u0xf00c29406ea45e19, ; 419: es/Microsoft.Maui.Controls.resources.dll => 6
	i64 u0xf038bf84c0c27e83, ; 420: lib_Microsoft.AspNetCore.SignalR.Client.dll.so => 38
	i64 u0xf09e47b6ae914f6e, ; 421: System.Net.NameResolution => 112
	i64 u0xf0de2537ee19c6ca, ; 422: lib_System.Net.WebHeaderCollection.dll.so => 118
	i64 u0xf11b621fc87b983f, ; 423: Microsoft.Maui.Controls.Xaml.dll => 52
	i64 u0xf1c4b4005493d871, ; 424: System.Formats.Asn1.dll => 103
	i64 u0xf238bd79489d3a96, ; 425: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 19
	i64 u0xf37221fda4ef8830, ; 426: lib_Xamarin.Google.Android.Material.dll.so => 84
	i64 u0xf3ddfe05336abf29, ; 427: System => 147
	i64 u0xf408654b2a135055, ; 428: System.Reflection.Emit.ILGeneration.dll => 126
	i64 u0xf437b94630896f3f, ; 429: lib_Plugin.Maui.Audio.dll.so => 57
	i64 u0xf4c1dd70a5496a17, ; 430: System.IO.Compression => 105
	i64 u0xf5fc7602fe27b333, ; 431: System.Net.WebHeaderCollection => 118
	i64 u0xf6077741019d7428, ; 432: Xamarin.AndroidX.CoordinatorLayout => 64
	i64 u0xf77b20923f07c667, ; 433: de/Microsoft.Maui.Controls.resources.dll => 4
	i64 u0xf7e2cac4c45067b3, ; 434: lib_System.Numerics.Vectors.dll.so => 121
	i64 u0xf7e74930e0e3d214, ; 435: zh-HK/Microsoft.Maui.Controls.resources.dll => 31
	i64 u0xf7fa0bf77fe677cc, ; 436: Newtonsoft.Json.dll => 56
	i64 u0xf84773b5c81e3cef, ; 437: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 29
	i64 u0xf8b77539b362d3ba, ; 438: lib_System.Reflection.Primitives.dll.so => 128
	i64 u0xf8e045dc345b2ea3, ; 439: lib_Xamarin.AndroidX.RecyclerView.dll.so => 79
	i64 u0xf915dc29808193a1, ; 440: System.Web.HttpUtility.dll => 143
	i64 u0xf91db4f1ad5d43b1, ; 441: Plugin.Maui.Audio.dll => 57
	i64 u0xf96c777a2a0686f4, ; 442: hi/Microsoft.Maui.Controls.resources.dll => 10
	i64 u0xf9eec5bb3a6aedc6, ; 443: Microsoft.Extensions.Options => 49
	i64 u0xfa3f278f288b0e84, ; 444: lib_System.Net.Security.dll.so => 116
	i64 u0xfa5ed7226d978949, ; 445: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 0
	i64 u0xfa645d91e9fc4cba, ; 446: System.Threading.Thread => 141
	i64 u0xfbf0a31c9fc34bc4, ; 447: lib_System.Net.Http.dll.so => 111
	i64 u0xfc6b7527cc280b3f, ; 448: lib_System.Runtime.Serialization.Formatters.dll.so => 132
	i64 u0xfc719aec26adf9d9, ; 449: Xamarin.AndroidX.Navigation.Fragment.dll => 76
	i64 u0xfd22f00870e40ae0, ; 450: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 68
	i64 u0xfd536c702f64dc47, ; 451: System.Text.Encoding.Extensions => 136
	i64 u0xfd583f7657b6a1cb, ; 452: Xamarin.AndroidX.Fragment => 69
	i64 u0xfda36abccf05cf5c, ; 453: System.Net.WebSockets.Client => 119
	i64 u0xfeae9952cf03b8cb, ; 454: tr/Microsoft.Maui.Controls.resources => 28
	i64 u0xff9b54613e0d2cc8 ; 455: System.Net.Http.Json => 110
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [456 x i32] [
	i32 81, i32 40, i32 77, i32 150, i32 60, i32 24, i32 2, i32 30,
	i32 114, i32 79, i32 93, i32 53, i32 31, i32 144, i32 63, i32 24,
	i32 91, i32 128, i32 68, i32 49, i32 91, i32 135, i32 140, i32 25,
	i32 87, i32 41, i32 82, i32 21, i32 151, i32 54, i32 112, i32 88,
	i32 67, i32 104, i32 120, i32 127, i32 79, i32 8, i32 149, i32 9,
	i32 45, i32 120, i32 39, i32 12, i32 137, i32 87, i32 18, i32 90,
	i32 147, i32 27, i32 150, i32 78, i32 16, i32 49, i32 104, i32 134,
	i32 126, i32 27, i32 141, i32 97, i32 65, i32 133, i32 8, i32 85,
	i32 50, i32 13, i32 11, i32 120, i32 149, i32 114, i32 29, i32 113,
	i32 100, i32 7, i32 139, i32 103, i32 33, i32 20, i32 127, i32 124,
	i32 142, i32 26, i32 138, i32 5, i32 46, i32 145, i32 69, i32 34,
	i32 62, i32 101, i32 8, i32 145, i32 6, i32 117, i32 53, i32 2,
	i32 51, i32 83, i32 42, i32 127, i32 128, i32 67, i32 112, i32 82,
	i32 1, i32 56, i32 136, i32 85, i32 144, i32 65, i32 75, i32 61,
	i32 151, i32 20, i32 89, i32 133, i32 85, i32 100, i32 24, i32 144,
	i32 22, i32 41, i32 122, i32 78, i32 138, i32 110, i32 74, i32 113,
	i32 119, i32 107, i32 125, i32 130, i32 14, i32 74, i32 150, i32 40,
	i32 140, i32 1, i32 51, i32 72, i32 102, i32 114, i32 98, i32 65,
	i32 55, i32 25, i32 113, i32 31, i32 134, i32 70, i32 92, i32 123,
	i32 148, i32 99, i32 15, i32 44, i32 89, i32 64, i32 142, i32 96,
	i32 37, i32 3, i32 36, i32 47, i32 118, i32 129, i32 63, i32 92,
	i32 137, i32 94, i32 145, i32 98, i32 5, i32 44, i32 86, i32 109,
	i32 52, i32 4, i32 130, i32 148, i32 84, i32 58, i32 51, i32 131,
	i32 97, i32 72, i32 66, i32 37, i32 3, i32 101, i32 103, i32 9,
	i32 129, i32 18, i32 55, i32 50, i32 66, i32 50, i32 76, i32 53,
	i32 2, i32 28, i32 18, i32 14, i32 94, i32 40, i32 11, i32 109,
	i32 42, i32 35, i32 80, i32 131, i32 58, i32 17, i32 27, i32 37,
	i32 69, i32 7, i32 95, i32 25, i32 4, i32 38, i32 17, i32 121,
	i32 93, i32 122, i32 96, i32 82, i32 43, i32 71, i32 57, i32 147,
	i32 33, i32 60, i32 62, i32 102, i32 29, i32 32, i32 46, i32 33,
	i32 42, i32 141, i32 104, i32 54, i32 86, i32 94, i32 74, i32 41,
	i32 99, i32 9, i32 66, i32 142, i32 90, i32 56, i32 75, i32 10,
	i32 23, i32 22, i32 21, i32 100, i32 34, i32 105, i32 72, i32 52,
	i32 39, i32 67, i32 138, i32 108, i32 1, i32 17, i32 105, i32 88,
	i32 6, i32 13, i32 55, i32 96, i32 90, i32 107, i32 77, i32 16,
	i32 59, i32 43, i32 19, i32 75, i32 71, i32 35, i32 135, i32 84,
	i32 78, i32 106, i32 16, i32 98, i32 132, i32 121, i32 135, i32 80,
	i32 68, i32 70, i32 12, i32 48, i32 125, i32 111, i32 45, i32 5,
	i32 107, i32 39, i32 131, i32 71, i32 146, i32 23, i32 140, i32 19,
	i32 143, i32 95, i32 116, i32 149, i32 122, i32 73, i32 26, i32 139,
	i32 3, i32 62, i32 10, i32 0, i32 106, i32 48, i32 101, i32 26,
	i32 148, i32 89, i32 22, i32 15, i32 146, i32 93, i32 117, i32 132,
	i32 111, i32 83, i32 61, i32 110, i32 0, i32 97, i32 108, i32 59,
	i32 15, i32 83, i32 119, i32 73, i32 46, i32 95, i32 115, i32 76,
	i32 125, i32 123, i32 38, i32 136, i32 143, i32 92, i32 28, i32 20,
	i32 23, i32 88, i32 34, i32 139, i32 86, i32 87, i32 36, i32 32,
	i32 115, i32 81, i32 99, i32 124, i32 70, i32 129, i32 61, i32 123,
	i32 47, i32 14, i32 45, i32 35, i32 73, i32 80, i32 81, i32 43,
	i32 48, i32 44, i32 91, i32 30, i32 126, i32 30, i32 108, i32 47,
	i32 134, i32 137, i32 32, i32 146, i32 11, i32 58, i32 60, i32 64,
	i32 36, i32 106, i32 130, i32 124, i32 116, i32 115, i32 13, i32 21,
	i32 12, i32 7, i32 133, i32 151, i32 102, i32 117, i32 109, i32 77,
	i32 63, i32 54, i32 59, i32 6, i32 38, i32 112, i32 118, i32 52,
	i32 103, i32 19, i32 84, i32 147, i32 126, i32 57, i32 105, i32 118,
	i32 64, i32 4, i32 121, i32 31, i32 56, i32 29, i32 128, i32 79,
	i32 143, i32 57, i32 10, i32 49, i32 116, i32 0, i32 141, i32 111,
	i32 132, i32 76, i32 68, i32 136, i32 69, i32 119, i32 28, i32 110
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 278e101698269c9bc8840aa94d72e7f24066a96d"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
