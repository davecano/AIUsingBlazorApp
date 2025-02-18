# AI For Everything 🤖

一个基于 Blazor 开发的现代化 AI 聊天应用。

## 功能特点

### 用户界面
- 现代化的响应式聊天界面
- 支持 Markdown 格式的消息显示
- 平滑的消息动画和滚动效果
- 自定义滚动条样式
- 优雅的错误提示 Toast 组件

### AI 对话
- 实时流式响应
- 支持 Markdown 格式输出
- 系统提示消息（隐藏显示）
- 完整的对话历史记录
- 自动滚动到最新消息

### AI 参数设置
- Temperature（温度）控制：调节输出的随机性
- Top P（采样阈值）：控制词汇采样范围
- Max Tokens（最大长度）：控制生成文本的最大长度
- Frequency Penalty（频率惩罚）：降低重复内容
- Presence Penalty（话题惩罚）：鼓励多样性

### 用户功能
- 用户登录/注册系统
- 个性化用户头像显示
- 聊天历史记录保存
- 一键退出登录

### 技术特性
- 基于 .NET 8 和 Blazor
- 实时流式响应
- 响应式设计
- 优雅的错误处理
- 状态管理
- 本地存储支持

## 开发中的功能
- [ ] 多语言支持
- [ ] 深色模式
- [ ] 导出聊天记录
- [ ] 更多 AI 模型支持

## 技术栈
- .NET 8
- Blazor
- Entity Framework Core
- Microsoft.SemanticKernel
- SQLite
- Markdig

## 开始使用

1. 克隆仓库
```bash
git clone [repository-url]
```

2. 配置环境
- 确保安装了 .NET 8 SDK
- 配置数据库连接字符串
- 设置 OpenAI API Key

3. 运行应用
```bash
dotnet run
```

## 贡献指南
欢迎提交 Pull Request 和 Issue。

## 许可证
[MIT License](LICENSE)

---

⭐ 如果这个项目对你有帮助，欢迎给它一个星标！ 