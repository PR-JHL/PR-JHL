# Pretext学习项目

这是一个用于学习Pretext文档编写的项目，同时为SolidWorks AI Script Assistant插件创建专业技术文档。

## 什么是Pretext？

Pretext是一个用于编写高质量技术文档的标记语言，特别适合：
- 📐 数学和科学文档
- 📚 技术书籍和教程
- 🧪 学术论文和报告
- 🔧 API文档和参考手册

## 项目结构

```
pretext/
├── project.ptx          # 项目主文件
├── frontmatter.ptx      # 前言部分（摘要、序言、致谢等）
├── introduction.ptx     # 第一章：引言
├── installation.ptx     # 第二章：安装指南
├── usage.ptx            # 第三章：使用指南
├── api-reference.ptx    # 第四章：API参考
├── examples.ptx         # 第五章：示例
├── appendix.ptx         # 附录
└── images/              # 图片资源目录
```

## 学习目标

通过这个项目，你将学习：

1. **Pretext基础语法**
   - 文档结构和组织
   - 段落和标题
   - 列表和表格
   - 代码块和语法高亮

2. **高级功能**
   - 数学公式（LaTeX支持）
   - 图表和图片
   - 交叉引用
   - 条件编译

3. **多格式输出**
   - HTML生成
   - PDF生成
   - EPUB生成

## 安装Pretext

### 使用pip安装（推荐）

```bash
pip install pretextbook
```

### 从源码安装

```bash
git clone https://github.com/PreTeXtBook/pretext.git
cd pretext
pip install -e .
```

## 基本命令

### 初始化项目

```bash
pretext init myproject
```

### 构建HTML

```bash
pretext build html
```

### 构建PDF

```bash
pretext build pdf
```

### 构建EPUB

```bash
pretext build epub
```

### 启动预览服务器

```bash
pretext serve
```

## Pretext语法示例

### 基本结构

```xml
<chapter>
  <title>章节标题</title>
  
  <section>
    <title>小节标题</title>
    
    <paragraph>
      这是一个段落。可以包含<strong>粗体</strong>和<em>斜体</em>文本。
    </paragraph>
    
    <list>
      <item>列表项1</item>
      <item>列表项2</item>
      <item>列表项3</item>
    </list>
  </section>
</chapter>
```

### 数学公式

```xml
<equation>
  E = mc^2
</equation>

<equation>
  \int_0^\infty e^{-x^2} dx = \frac{\sqrt{\pi}}{2}
</equation>
```

### 代码块

```xml
<code-block language="python">
def hello():
    print("Hello, Pretext!")
</code-block>
```

### 表格

```xml
<table>
  <thead>
    <tr>
      <th>列1</th>
      <th>列2</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>数据1</td>
      <td>数据2</td>
    </tr>
  </tbody>
</table>
```

### 警告和提示

```xml
<warning>
  <paragraph>这是一个警告信息</paragraph>
</warning>

<note>
  <paragraph>这是一个提示信息</paragraph>
</note>
```

### 步骤说明

```xml
<procedure>
  <step>
    <title>第一步</title>
    <paragraph>描述第一步的操作</paragraph>
  </step>
  <step>
    <title>第二步</title>
    <paragraph>描述第二步的操作</paragraph>
  </step>
</procedure>
```

## 项目构建

### 构建HTML输出

```bash
cd pretext
pretext build html
```

构建结果将输出到 `../output/html/` 目录。

### 启动预览

```bash
pretext serve --port 8000
```

然后在浏览器中访问 `http://localhost:8000`。

## 学习路径

1. **入门**：从 `project.ptx` 和 `frontmatter.ptx` 开始，了解项目结构
2. **基础**：学习 `introduction.ptx` 中的基本语法
3. **进阶**：查看 `installation.ptx` 中的步骤说明和警告框
4. **代码**：学习 `api-reference.ptx` 中的代码块
5. **数学**：学习 `introduction.ptx` 中的数学公式
6. **示例**：参考 `examples.ptx` 中的完整示例

## 资源推荐

### 官方文档
- [Pretext官方网站](https://pretextbook.org/)
- [Pretext指南](https://pretextbook.org/doc/guide/html/index.html)
- [Pretext参考手册](https://pretextbook.org/doc/reference/html/index.html)

### 教程和示例
- [Pretext示例仓库](https://github.com/PreTeXtBook/pretext-examples)
- [Pretext教程项目](https://pretextbook.org/tutorial/)

### 社区
- [Pretext讨论组](https://github.com/PreTeXtBook/pretext/discussions)
- [Stack Exchange](https://tex.stackexchange.com/questions/tagged/pretext)

## 常见问题

### Q: Pretext与Markdown有什么区别？

A: Markdown适合快速编写简单文档，而Pretext专为技术文档设计，支持复杂的结构、数学公式、交叉引用等高级功能。

### Q: 如何添加图片？

A: 将图片放入 `images/` 目录，然后使用：

```xml
<figure>
  <image source="images/myimage.png" width="70%" />
  <caption>图片标题</caption>
</figure>
```

### Q: 如何生成PDF？

A: 需要安装TeX发行版（如TeX Live或MiKTeX），然后运行：

```bash
pretext build pdf
```

### Q: 如何添加交叉引用？

A: 使用 `@` 符号引用标签：

```xml
<section xml:id="my-section">
  <title>我的章节</title>
  ...
</section>

<paragraph>
  请参见<section ref="my-section"/>了解更多信息。
</paragraph>
```

## 许可证

本项目采用 CC BY-NC-SA 4.0 许可证。

---

**开始你的Pretext学习之旅吧！** 🚀
