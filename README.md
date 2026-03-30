# Headless Multi-Tenant E-Commerce SaaS Platform

**Personal Project | 2025 – Present**

A production-oriented SaaS platform modeling real-world multi-tenant commerce systems, including tenant isolation, modular services, and scalable infrastructure. Demonstrates modern frontend and backend patterns, authentication, testing, and CI/CD practices.

---

## Features

- **Admin CMS & Marketplace**: Built with Next.js (App Router) in a monorepo, sharing UI components and utility packages for consistency and reuse
- **Multi-Tenant Backend**: .NET APIs with Onion Architecture, EF Core, and PostgreSQL (Neon)
- **Authentication & Authorization**: Clerk as an external identity provider, with JWT-based authentication, claims transformation, and policy-based authorization
- **System Reliability & Observability**: Structured logging, correlation IDs, centralized error handling, health checks, and Swagger API documentation
- **CI/CD & DevOps**: GitHub Actions pipelines deploying to Vercel (frontend) and Railway (Dockerized backend), including database migrations via Neon PostgreSQL. Branch protection rules enforced for production-quality code  
- **Testing**: Unit, integration, and component tests using Vitest (frontend) and xUnit (backend)  
- **Ongoing Development**: Expanding core domain features, store management APIs, and test coverage to improve system stability and scalability

---

## Architecture & Tech Stack

### Frontend
- Next.js 16 | TypeScript | Zustand | Zod | TailwindCSS | shadcn/ui | Clerk | Vitest  

### Backend
- .NET | C# | EF Core | PostgreSQL (Neon) | JWT | xUnit  

### DevOps & Tools
- Git | GitHub Actions | Vercel | Railway  

---

## 📌 Notes

- This project is **personal and educational**; all code is written to demonstrate system design, architecture, and modern software development practices  
- No sensitive or client data is included; all examples are based on personal development and learning  

---

## 🔗 Links

- Portfolio: [ronelmayor.dev](https://ronelmayor.dev)  
- LinkedIn: [linkedin.com/in/ronelmayor](https://linkedin.com/in/ronelmayor)
