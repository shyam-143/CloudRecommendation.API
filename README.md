# ☁️ Cloud Services Recommendation Engine (GenAI)

An intelligent, full-stack web application that helps businesses select the best-fit cloud services (Compute, Storage, Database) across AWS, Azure, and GCP. The platform collects specific user requirements via an interactive questionnaire and leverages **Azure OpenAI (GPT-4.1)** to generate tailored recommendations, feature comparisons, and monthly cost estimates.

## 🚀 Key Features

- **Interactive Requirement Gathering:** Form-based input to collect workload, usage, compliance, and technical specs.
- **GenAI-Powered Recommendations:** Utilizes Azure OpenAI (GPT-4.1) to analyze requirements and generate intelligent, context-aware cloud service recommendations.
- **Cost & Feature Comparison:** Displays structured monthly cost estimates and feature comparisons in an interactive UI table.
- **User History & State:** Secure user registration, profile management, and persistent search history.
- **Enterprise Security:** Secure user data handling with audit logging and Role-Based Access Control (RBAC).

## ⚙️ Tech Stack

- **Backend:** .NET 8, C#, ASP.NET Core Web API
- **Frontend:** React.js, TypeScript
- **Database:** SQL Server (T-SQL, Entity Framework Core)
- **Gen AI:** Azure OpenAI Service (GPT-4.1)
- **Tools:** Git, GitHub, Postman, Visual Studio

## 📊 Performance & Non-Functional Requirements (NFRs)

Engineered to meet strict enterprise performance and scalability standards:
- **Performance:** Web load < 3s | API response 1-2s | SQL queries < 500ms.
- **Concurrency:** Optimized to seamlessly support 15+ concurrent users with zero degradation.
- **Scalability:** Architected to handle increasing user load and expanding cloud provider data.
- **Security:** Implemented secure authentication, data encryption, and comprehensive audit logging.

## 🤖 AI Architecture & The Path to "Agents"

This project serves as a foundational **AI-driven recommendation engine**. 
- **Current Implementation:** Uses advanced prompt engineering and structured JSON generation via Azure OpenAI to map user constraints to cloud provider capabilities and pricing models.
- **Agentic Evolution:** To transition this into a fully **Autonomous Agent**, the architecture is designed to integrate **Function Calling (Tool Use)**. This would allow the LLM to dynamically query live cloud pricing APIs (e.g., AWS Pricing Calculator API) and fetch real-time compliance data before generating its final recommendation, moving from a "prompt-response" model to an "act-and-observe" agentic loop.

## 📁 Project Structure

```text
CloudRecommendation.API/
├── CloudRecommendation.API/       # .NET 8 Backend (Controllers, AI Services, DB Context)
├── cloud-recommendation-ui/       # React Frontend (Components, State, API Integration)
└── README.md                      # Project Documentation
