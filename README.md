# SolidWorks NLP Modeling Plugin

基于自然语言的SolidWorks建模插件，支持使用日常语言进行3D建模操作。

## 功能特性

- **自然语言交互**：使用自然语言描述你的建模需求
- **本地LLM集成**：支持本地部署的大语言模型，数据安全无忧
- **基础几何建模**：支持创建立方体、球体、圆柱、圆锥、圆环、金字塔等基本几何体
- **特征操作**：支持拉伸、旋转等建模操作
- **用户友好界面**：集成在SolidWorks任务面板中的直观界面
- **SolidWorks 2022+ 支持**：兼容SolidWorks 2022及以上版本

## 项目结构

```
/workspace/
├── SolidWorksNLPPlugin.sln           # Visual Studio解决方案文件
├── README.md                          # 项目说明文档
└── SolidWorksNLPPlugin/              # 主项目文件夹
    ├── SolidWorksNLPPlugin.csproj    # 项目配置文件
    ├── Plugin.cs                     # 插件入口点
    ├── NlpCommandHandler.cs          # NLP命令处理器
    ├── LocalLlmClient.cs             # 本地LLM客户端
    ├── ModelBuilder.cs               # 模型构建器
    ├── MainPanel.cs                  # 用户界面
    └── Properties/
        └── AssemblyInfo.cs           # 程序集信息
```

## 开发环境要求

- **操作系统**：Windows 10/11 (64位)
- **开发工具**：Visual Studio 2022 (或更高版本)
- **.NET Framework**：.NET Framework 4.8
- **SolidWorks**：SolidWorks 2022或更高版本
- **本地LLM服务**：Ollama (推荐) 或其他兼容OpenAI API的本地LLM服务

## 安装与配置

### 1. 获取SolidWorks API程序集

由于SolidWorks API程序集是商业软件的一部分，你需要从SolidWorks安装目录获取以下文件：

- `SolidWorks.Interop.sldworks.dll`
- `SolidWorks.Interop.swconst.dll`
- `SolidWorks.Interop.swdocumentmgr.dll`

这些文件通常位于：
```
C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\
```

将这些文件复制到项目的 `SolidWorksNLPPlugin/lib/` 文件夹中。

### 2. 安装本地LLM服务 (Ollama)

1. 下载并安装Ollama：https://ollama.ai/
2. 安装所需模型（推荐使用llama2或类似模型）：
```bash
ollama pull llama2
```
3. 确保Ollama服务正在运行（默认监听 `http://localhost:11434`）

### 3. 编译项目

1. 使用Visual Studio打开 `SolidWorksNLPPlugin.sln`
2. 选择 `x64` 平台配置（重要！SolidWorks 64位需要）
3. 选择 `Release` 配置
4. 点击「生成」→「生成解决方案」

### 4. 注册插件

1. 以管理员身份运行Visual Studio命令提示符
2. 导航到编译输出目录（通常是 `bin/x64/Release/`）
3. 使用regasm注册：
```bash
regasm /codebase /tlb:SolidWorksNLPPlugin.tlb SolidWorksNLPPlugin.dll
```

### 5. 配置SolidWorks

1. 启动SolidWorks
2. 打开「工具」→「插件」
3. 在列表中找到「SolidWorks NLP Modeling Plugin」并勾选
4. 点击「确定」

## 使用说明

### 基础操作

1. 启动SolidWorks并加载插件后，在任务面板中会出现「NLP Modeling」面板
2. 在文本框中输入自然语言命令，例如：
   - "创建一个100mm的立方体"
   - "创建一个直径50mm、高度200mm的圆柱"
   - "拉伸草图50mm"
3. 点击「Send」按钮执行命令

### 支持的命令类型

| 命令类型 | 示例 |
|---------|------|
| 创建基本体 | "创建一个100mm的立方体" |
| 创建球体 | "创建一个直径100mm的球体" |
| 创建圆柱 | "创建一个直径50mm、高度200mm的圆柱" |
| 创建圆锥 | "创建一个圆锥" |
| 拉伸 | "拉伸50mm" |
| 旋转 | "旋转360度" |

### 参数说明

- 尺寸参数默认为毫米(mm)
- 如果没有指定尺寸，会使用默认值

## 自定义配置

### 修改LLM API地址

在 `LocalLlmClient.cs` 中修改以下代码：

```csharp
_apiUrl = "http://localhost:11434/api/chat";  // 修改为你的LLM API地址
_modelName = "llama2";  // 修改为你的模型名称
```

### 使用其他LLM服务

如果使用其他兼容OpenAI API格式的服务，只需修改 `SendChatRequestAsync` 方法中的请求格式即可。

## 开发扩展

### 添加新的建模功能

1. 在 `ModelBuilder.cs` 中添加新的方法
2. 在 `NlpCommandHandler.cs` 中更新命令处理逻辑
3. 在 `LocalLlmClient.cs` 中添加对应的提示词和解析规则

### 添加新的命令类型

1. 在 `CommandType` 枚举中添加新类型
2. 更新LLM提示词，让模型识别新命令
3. 实现对应的命令执行逻辑

## 常见问题

### Q: 插件没有出现在SolidWorks中？

A: 请确认：
- 使用了x64平台编译
- 正确使用regasm注册了dll
- SolidWorks版本是2022或更高

### Q: LLM连接失败？

A: 请确认：
- Ollama服务正在运行
- API地址和模型名称配置正确
- 防火墙没有阻止本地连接

### Q: 为什么使用本地LLM而不是云端API？

A: 使用本地LLM的优势：
- 数据安全：模型和数据都在本地
- 无网络依赖：离线可用
- 成本低廉：无需支付API费用
- 响应速度快：低延迟

## 技术架构

```
用户输入
  ↓
MainPanel (UI层)
  ↓
NlpCommandHandler (命令处理层)
  ↓
LocalLlmClient (自然语言理解层)
  ↓
ModelBuilder (建模执行层)
  ↓
SolidWorks API (底层建模)
```

## 未来计划

- [ ] 支持更复杂的草图绘制
- [ ] 支持装配体操作
- [ ] 支持参数化建模
- [ ] 添加更多本地LLM模型支持
- [ ] 添加历史命令记录
- [ ] 支持命令模板保存
- [ ] 添加更多示例和教程

## 许可证

本项目仅供学习和研究使用。

## 贡献

欢迎提交Issue和Pull Request！

## 免责声明

本项目与Dassault Systèmes SolidWorks Corporation无关。SolidWorks是其注册商标。

---

**注意**：本项目需要SolidWorks软件才能正常运行。
