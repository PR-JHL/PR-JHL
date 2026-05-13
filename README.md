# SolidWorks AI Script Assistant

一个基于云端AI API的SolidWorks脚本生成插件，支持使用自然语言描述来生成SolidWorks操作脚本，并可保存为可复用的Skills。

## 功能特性

- **云端AI集成**：使用OpenAI API或兼容API进行智能脚本生成
- **脚本动态编译执行**：实时编译并执行生成的C#脚本
- **Skill保存系统**：将常用操作保存为可复用的Skills
- **用户友好界面**：内置SolidWorks任务面板，支持聊天和Skills管理
- **SolidWorks 2022+ 支持**：兼容SolidWorks 2022及以上版本

## 工作流程

1. **配置API**：设置你的OpenAI API密钥和模型
2. **描述需求**：用自然语言描述你想在SolidWorks中执行的操作
3. **生成脚本**：AI自动生成SolidWorks API C#脚本
4. **执行脚本**：一键执行生成的脚本
5. **保存为Skill**：将有用的脚本保存为可复用的Skill

## 项目结构

```
/workspace/
├── SolidWorksNLPPlugin.sln              # Visual Studio解决方案文件
├── README.md                            # 项目说明文档
└── SolidWorksNLPPlugin/                 # 主项目文件夹
    ├── SolidWorksNLPPlugin.csproj       # 项目配置文件
    ├── Plugin.cs                        # SolidWorks插件入口
    ├── NlpCommandHandler.cs             # 命令处理器
    ├── CloudLlmClient.cs                # 云端AI API客户端
    ├── ScriptExecutor.cs                # 脚本编译器和执行器
    ├── SkillManager.cs                  # Skill管理系统
    ├── MainPanel.cs                     # 用户界面
    └── Properties/
        └── AssemblyInfo.cs              # 程序集信息
```

## 开发环境要求

- **操作系统**：Windows 10/11 (64位)
- **开发工具**：Visual Studio 2022 (或更高版本)
- **.NET Framework**：.NET Framework 4.8
- **SolidWorks**：SolidWorks 2022或更高版本
- **API服务**：OpenAI API密钥（或兼容的API服务）

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

将这些文件复制到项目的 `SolidWorksNLPPlugin\lib\` 文件夹中。

### 2. 配置API密钥

编译并安装插件后，在插件界面配置：
- **API Key**：你的OpenAI API密钥
- **API URL**：API端点（默认为OpenAI官方API）
- **Model**：使用的模型（默认为gpt-4）

### 3. 编译项目

1. 使用Visual Studio打开 `SolidWorksNLPPlugin.sln`
2. 选择 `x64` 平台配置（重要！SolidWorks 64位需要）
3. 选择 `Release` 配置
4. 点击「生成」→「生成解决方案」

### 4. 注册插件

1. 以管理员身份运行Visual Studio命令提示符
2. 导航到编译输出目录（通常是 `bin\x64\Release\`）
3. 使用regasm注册：
```bash
regasm /codebase /tlb:SolidWorksNLPPlugin.tlb SolidWorksNLPPlugin.dll
```

### 5. 配置SolidWorks

1. 启动SolidWorks
2. 打开「工具」→「插件」
3. 在列表中找到「SolidWorks AI Script Assistant」并勾选
4. 点击「确定」

## 使用说明

### 基本使用

1. 启动SolidWorks并加载插件后，在任务面板中找到「AI Assistant」标签
2. 配置你的API密钥
3. 在文本框中描述你想做的事情，例如：
   - "创建一个100mm的立方体"
   - "创建一个直径50mm、高度200mm的圆柱"
   - "在原点处创建一个草图，画一个半径50mm的圆"
4. 点击「Generate Script」生成脚本
5. 预览生成的脚本后点击「Execute Script」执行
6. 如需保存此操作，点击「Save as Skill」

### Skill管理

1. 切换到「Skills」标签
2. 查看已保存的所有Skills
3. 双击或点击「Use Selected Skill」使用选中的Skill
4. 点击「Delete Selected」删除不再需要的Skill
5. 点击「Refresh」刷新列表

### 示例提示词

```
创建一个新零件，在前视基准面上画一个边长100mm的正方形草图，然后拉伸50mm
```

```
创建一个直径80mm、高度120mm的圆柱，然后在顶面创建一个直径40mm的同轴圆柱拉伸切除
```

```
在当前激活的零件中，选择前视基准面，创建一个包含多个同心圆的草图
```

## 自定义配置

### 使用其他AI服务

如果使用兼容OpenAI API格式的其他服务，只需修改「API URL」配置项即可。

### 支持的模型

- gpt-4（推荐，生成质量最好）
- gpt-3.5-turbo
- 其他兼容OpenAI API格式的模型

## 安全说明

- API密钥仅保存在内存中，不持久化存储
- 脚本执行前可预览，确认无误后再执行
- 建议在测试环境中先验证脚本
- Skills保存在用户目录下，不包含敏感信息

## 技术架构

```
用户自然语言输入
    ↓
MainPanel (UI层)
    ↓
NlpCommandHandler (业务逻辑层)
    ↓
CloudLlmClient (AI API调用层)
    ↓
ScriptExecutor (脚本编译执行层)
    ↓
SolidWorks API (底层建模)
    ↓
SkillManager (可选保存操作)
```

## 常见问题

### Q: 插件没有出现在SolidWorks中？

A: 请确认：
- 使用了x64平台编译
- 正确使用regasm注册了dll
- SolidWorks版本是2022或更高

### Q: API调用失败？

A: 请确认：
- API密钥正确且有效
- 网络连接正常
- API URL配置正确
- 账户有足够的额度

### Q: 脚本执行出错？

A: 可能的原因：
- AI生成的脚本有错误，可以尝试重新生成
- 缺少必要的前置条件（如没有激活的零件）
- SolidWorks API调用方式有问题

### Q: Skills保存在哪里？

A: Skills保存在用户的应用数据目录下：
```
%APPDATA%\SolidWorksNLPPlugin\Skills\
```

## 未来计划

- [ ] 添加脚本编辑功能
- [ ] 支持脚本参数化
- [ ] 添加更多示例模板
- [ ] 支持批量执行Skills
- [ ] 添加Skill分享功能
- [ ] 支持Undo/Redo记录
- [ ] 添加脚本语法高亮

## 贡献

欢迎提交Issue和Pull Request！

## 免责声明

本项目与Dassault Systèmes SolidWorks Corporation无关。SolidWorks是其注册商标。使用本插件生成的脚本请自行验证，作者不对使用结果负责。

## 许可证

本项目仅供学习和研究使用。

---

**注意**：本项目需要SolidWorks软件和有效的AI API密钥才能正常运行。
