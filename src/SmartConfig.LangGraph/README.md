# 🤖 LangGraph Content Creation Supervisor

A streamlined, LLM-powered content creation workflow using LangGraph that automatically generates high-quality blog posts through intelligent agent coordination.

## 🚀 Features

- **🤖 LLM Supervisor**: Intelligent routing between workflow stages
- **📝 Content Planning**: Strategic content strategy and outline creation
- **🔬 Research Agent**: Web search and data gathering with Tavily
- **✍️ Writer Agent**: Complete blog posts with facts, citations, and SEO optimization
- **📄 Publisher**: Automatic content saving to markdown files
- **⚡ Fast Execution**: 4-step streamlined workflow (3x faster than traditional approaches)

## 🏗️ Architecture

```
User Input → LLM Supervisor → [Plan → Research → Write → Publish] → Final Blog Post
```

### Workflow Stages:
1. **Content Planner**: Creates content strategy and outline
2. **Research Agent**: Conducts web research and gathers information
3. **Writer Agent**: Writes complete blog post with facts, citations, and SEO
4. **Publisher**: Saves final content to markdown file

## 🛠️ Installation

### Prerequisites
- Python 3.9+
- OpenAI API key
- Tavily API key (optional, for web research)

### Setup

1. **Clone the repository**
```bash
git clone https://github.com/AiAgentGuy/LangGraph-supervisor.git
cd SmartConfig.LangGraph
```

2. **Create and activate virtual environment**
```bash
python3 -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
```

3. **Install dependencies**
```bash
pip install -r requirements.txt
```

4. **Configure environment variables**
```bash
# Create .env file
cp env.example .env

# Edit .env file with your API keys
OPENAI_API_KEY=sk-your-openai-key-here
TAVILY_API_KEY=tvly-your-tavily-key-here  # Optional
OPENAI_MODEL=gpt-4o-mini  # Optional, defaults to gpt-5-nano-2025-08-07
```

## 🚀 Usage

### Interactive Mode
```bash
python -m src.main
```
Then enter your content creation request when prompted.

### Programmatic Usage
```python
from src.main import create_content

result = create_content("Create a blog post about AI in healthcare")
final_content = result['messages'][-1].content
print(final_content)
```

### Example Requests
- "Create a blog post about renewable energy benefits"
- "Write an article about remote work productivity"
- "Create content about sustainable living practices"

## 📁 Project Structure

```
SmartConfig.LangGraph/
├── src/
│   ├── agents/           # Workflow agents
│   │   ├── content_planner.py
│   │   ├── research_agent.py
│   │   ├── writer_agent.py
│   │   └── publisher.py
│   ├── utils/            # Utilities and configuration
│   │   ├── config.py
│   │   └── model.py
│   ├── main.py           # Main entry point
│   └── supervisor_graph.py  # LangGraph workflow definition
├── outputs/              # Generated blog posts
├── requirements.txt      # Python dependencies
└── README.md
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `OPENAI_API_KEY` | OpenAI API key for LLM access | ✅ | - |
| `TAVILY_API_KEY` | Tavily API key for web research | ❌ | - |
| `OPENAI_MODEL` | OpenAI model to use | ❌ | `gpt-5-nano-2025-08-07` |

### Customization

You can customize the workflow by modifying:
- **Agent prompts**: Edit the system prompts in each agent file
- **Workflow stages**: Modify `supervisor_graph.py` to add/remove agents
- **Output format**: Adjust the publisher to save in different formats

## 📊 Performance

- **Execution Time**: ~40 seconds for a complete blog post
- **Content Quality**: 800-1200 words with facts, citations, and SEO
- **Workflow Steps**: 4 streamlined steps (vs 7 in traditional approaches)

## 🔒 Security

- ✅ No hardcoded credentials
- ✅ Environment variables for all API keys
- ✅ Comprehensive `.gitignore` file
- ✅ Secure credential handling

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [LangGraph](https://github.com/langchain-ai/langgraph) for the workflow framework
- [OpenAI](https://openai.com/) for LLM capabilities
- [Tavily](https://tavily.com/) for web search functionality

## 📞 Support

If you encounter any issues or have questions:
1. Check the [Issues](https://github.com/your-username/LangGraph-Supervisor/issues) page
2. Create a new issue with detailed information
3. Include your environment details and error messages

---

**Made with ❤️ using LangGraph and OpenAI**


