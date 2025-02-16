# AI For Everything 🤖

一个现代化的 AI 聊天应用，基于 Blazor 构建，提供流畅的对话体验。

![AI Assistant Preview](docs/images/preview.png)

## ✨ 特性

- 🚀 实时流式响应 - 像真实对话一样，一个字一个字地显示 AI 的回答
- 💬 优雅的聊天界面 - 现代化的 UI 设计，支持响应式布局
- 🎨 渐变色主题 - 精心设计的渐变色彩，让界面更加生动
- 📱 移动端适配 - 完美支持各种设备尺寸
- 🔄 聊天历史记录 - 自动保存对话历史
- 🎭 支持模拟模式 - 便于开发和测试

## 🛠️ 技术栈

- **.NET 8** - 最新的 .NET 平台
- **Blazor** - 现代化的 Web UI 框架
- **Semantic Kernel** - 强大的 AI 开发框架
- **OpenAI API** - 领先的 AI 模型接口

## 🚀 快速开始

1. 克隆仓库：
```bash
git clone [repository-url]
```

2. 配置 OpenAI 设置：
   - 打开 `appsettings.json`
   - 设置你的 API Key 和其他参数
   - 或使用模拟模式进行测试

3. 运行项目：
```bash
dotnet run
```

## 🎯 项目架构

项目采用模板方法设计模式，通过抽象基类 `BaseChatService` 实现核心功能：

- 📝 `IAIChatService` - 定义聊天服务接口
- 🏗️ `BaseChatService` - 实现模板方法模式的抽象基类
- 🤖 `AIChatService` - 实际的 AI 聊天实现
- 🎭 `MockAIChatService` - 用于开发测试的模拟实现

## 🎨 界面预览

应用提供了精美的用户界面：

- 渐变色标题栏
- 流畅的消息动画
- 响应式布局设计
- 优雅的滚动条样式
- 现代化的输入框设计

## 🔧 配置选项

在 `appsettings.json` 中可以配置：

```json
{
  "UseMockService": true,  // 是否使用模拟服务
  "OpenAISettings": {
    "ModelId": "your-model",
    "ApiKey": "your-api-key",
    "Endpoint": "your-endpoint"
  }
}
```

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

[MIT License](LICENSE)

---

⭐ 如果这个项目对你有帮助，欢迎给它一个星标！ 