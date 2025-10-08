# 🛒 FlexiMarket

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Architecture](https://img.shields.io/badge/architecture-Clean%20Architecture-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![CQRS](https://img.shields.io/badge/pattern-CQRS-orange)](https://martinfowler.com/bliki/CQRS.html)
[![DDD](https://img.shields.io/badge/design-DDD-blue)](https://martinfowler.com/tags/domain%20driven%20design.html)

**FlexiMarket** est une solution de microservice e-commerce moderne construite avec ASP.NET Core, implémentant les meilleures pratiques d'architecture logicielle : **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, et les principes **SOLID**.

---

## 🎯 Vision du Projet

FlexiMarket démontre comment construire un système de gestion de commandes e-commerce robuste, scalable et maintenable en appliquant des patterns architecturaux éprouvés. Ce projet sert à la fois de référence et de base solide pour développer des microservices complexes.

## ✨ Caractéristiques Principales

### 🏗️ Architecture

- **Clean Architecture** - Séparation stricte en couches (Domain, Application, Infrastructure, API)
- **Domain-Driven Design** - Modélisation riche du domaine métier avec agrégats, entités et value objects
- **CQRS Pattern** - Séparation des commandes (write) et des requêtes (read)
- **Event-Driven** - Communication asynchrone via domain events
- **Repository Pattern** - Abstraction de la couche de persistance

### 🎨 Principes SOLID

- ✅ **S**ingle Responsibility - Une classe, une responsabilité
- ✅ **O**pen/Closed - Ouvert à l'extension, fermé à la modification
- ✅ **L**iskov Substitution - Substitution transparente des abstractions
- ✅ **I**nterface Segregation - Interfaces spécifiques et ciblées
- ✅ **D**ependency Inversion - Dépendance vers les abstractions

### 💼 Use Cases Métier Complexes

- Création et gestion du cycle de vie des commandes
- Validation multi-étapes avec services externes
- Gestion des stocks et réservations d'inventaire
- Pré-autorisation et capture de paiements
- Gestion des événements de domaine et audit trail

---

## 📁 Structure du Projet

```
FlexiMarket/
├── src/
│   ├── FlexiMarket.Domain/              # 🎯 Cœur métier
│   │   ├── Entities/                    # Agrégats et entités
│   │   │   ├── Order.cs
│   │   │   └── OrderLine.cs
│   │   ├── ValueObjects/                # Objets valeur immuables
│   │   │   ├── Money.cs
│   │   │   ├── Address.cs
│   │   │   └── CustomerId.cs
│   │   ├── Events/                      # Événements de domaine
│   │   │   └── OrderEvents.cs
│   │   └── Base/                        # Classes de base
│   │       ├── Entity.cs
│   │       ├── AggregateRoot.cs
│   │       └── ValueObject.cs
│   │
│   ├── FlexiMarket.Application/         # 🔧 Use Cases
│   │   ├── UseCases/
│   │   │   ├── Commands/                # Commandes (write)
│   │   │   │   ├── CreateOrderCommand.cs
│   │   │   │   └── SubmitOrderCommand.cs
│   │   │   └── Queries/                 # Requêtes (read)
│   │   │       └── GetOrderDetailsQuery.cs
│   │   └── Interfaces/                  # Contrats
│   │       ├── ICommand.cs
│   │       ├── IQuery.cs
│   │       └── Repositories/
│   │
│   ├── FlexiMarket.Infrastructure/      # 🔌 Implémentation
│   │   ├── Persistence/                 # Base de données
│   │   │   ├── OrderDbContext.cs
│   │   │   └── Repositories/
│   │   ├── Services/                    # Services externes
│   │   │   ├── InventoryService.cs
│   │   │   ├── PricingService.cs
│   │   │   └── PaymentService.cs
│   │   └── Messaging/                   # Event Bus
│   │       └── RabbitMqEventBus.cs
│   │
│   └── FlexiMarket.API/                 # 🌐 API REST
│       ├── Controllers/
│       │   └── OrdersController.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   ├── FlexiMarket.Domain.Tests/
│   ├── FlexiMarket.Application.Tests/
│   └── FlexiMarket.API.Tests/
│
└── docs/
    ├── architecture.md
    └── use-cases.md
```

---

## 🚀 Technologies Utilisées

### Backend
- **ASP.NET Core 8.0** - Framework web moderne
- **Entity Framework Core** - ORM pour la persistance
- **MediatR** - Pattern Mediator pour CQRS
- **FluentValidation** - Validation des commandes
- **AutoMapper** - Mapping objet-objet

### Base de Données
- **SQL Server** - Base de données relationnelle
- **Redis** - Cache distribué

### Messaging
- **RabbitMQ** - Message broker pour les événements
- **MassTransit** - Framework de messaging

### Testing
- **xUnit** - Framework de tests
- **Moq** - Mocking framework
- **FluentAssertions** - Assertions lisibles

### DevOps
- **Docker** - Conteneurisation
- **Docker Compose** - Orchestration locale
- **Swagger/OpenAPI** - Documentation API

---

## 🏃 Getting Started

### Prérequis

```bash
- .NET 8.0 SDK
- SQL Server 2019+ ou Docker
- RabbitMQ (optionnel pour les événements)
- Visual Studio 2022 / VS Code / Rider
```

### Installation

1. **Cloner le repository**
```bash
git clone https://github.com/votre-username/fleximarket.git
cd fleximarket
```

2. **Restaurer les dépendances**
```bash
dotnet restore
```

3. **Configurer la base de données**
```bash
# Mettre à jour la connection string dans appsettings.json
cd src/FlexiMarket.API
dotnet ef database update
```

4. **Lancer l'application**
```bash
dotnet run --project src/FlexiMarket.API
```

5. **Accéder à Swagger**
```
https://localhost:5001/swagger
```

### Utilisation avec Docker

```bash
# Lancer tous les services
docker-compose up -d

# L'API sera disponible sur http://localhost:8080
```

---

## 📖 Exemples d'Utilisation

### Créer une commande

```bash
POST /api/orders
Content-Type: application/json

{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "shippingAddress": {
    "street": "123 Rue de la Paix",
    "city": "Paris",
    "postalCode": "75001",
    "country": "France"
  }
}
```

### Ajouter des produits

```bash
POST /api/orders/{orderId}/items
Content-Type: application/json

{
  "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "quantity": 2,
  "unitPrice": {
    "amount": 29.99,
    "currency": "EUR"
  }
}
```

### Soumettre une commande

```bash
POST /api/orders/{orderId}/submit
```

### Récupérer les détails

```bash
GET /api/orders/{orderId}
```

---

## 🧪 Tests

### Lancer tous les tests

```bash
dotnet test
```

### Tests par projet

```bash
# Tests unitaires du domaine
dotnet test tests/FlexiMarket.Domain.Tests

# Tests d'intégration
dotnet test tests/FlexiMarket.API.Tests
```

### Couverture de code

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 🎓 Concepts Implémentés

### Domain-Driven Design

- **Agrégats** : `Order` est un agrégat qui contrôle `OrderLine`
- **Value Objects** : `Money`, `Address` - objets immuables
- **Domain Events** : Communication entre agrégats
- **Ubiquitous Language** : Vocabulaire métier partagé

### CQRS Pattern

- **Commands** : Modifications d'état via handlers dédiés
- **Queries** : Lecture optimisée avec DTOs
- **Séparation** : Modèles write et read différents

### Event-Driven Architecture

- **Domain Events** : `OrderCreatedEvent`, `OrderSubmittedEvent`
- **Event Handlers** : Traitement asynchrone
- **Event Sourcing Ready** : Architecture préparée pour l'event sourcing

---

## 🛡️ Bonnes Pratiques

### Sécurité
- ✅ Validation des entrées avec FluentValidation
- ✅ Protection CSRF
- ✅ Rate limiting
- ✅ Authentication JWT (à implémenter)

### Performance
- ✅ Async/await partout
- ✅ Pagination des résultats
- ✅ Cache Redis pour les lectures
- ✅ Indexation des requêtes fréquentes

### Qualité du Code
- ✅ Respect des principes SOLID
- ✅ Couverture de tests > 80%
- ✅ Code reviews systématiques
- ✅ Analyse statique avec SonarQube

---

## 📈 Roadmap

### v1.0 (Actuel)
- [x] Architecture de base
- [x] Gestion des commandes
- [x] CQRS Pattern
- [x] Domain Events

### v1.1 (Prochain)
- [ ] Authentication & Authorization
- [ ] API Gateway avec Ocelot
- [ ] Service de catalogue produits
- [ ] Elasticsearch pour la recherche

### v2.0 (Futur)
- [ ] Event Sourcing complet
- [ ] Saga Pattern pour les transactions distribuées
- [ ] GraphQL API
- [ ] Microservices additionnels (Inventory, Payment, Shipping)

---

## 🤝 Contribution

Les contributions sont les bienvenues ! Veuillez suivre ces étapes :

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit les changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

### Guidelines

- Respecter les conventions de code C#
- Ajouter des tests pour toute nouvelle fonctionnalité
- Mettre à jour la documentation
- S'assurer que tous les tests passent

---

## 📝 License

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

---

## 👥 Auteurs

- **Votre Nom** - *Travail initial* - [YourGitHub](https://github.com/yourusername)

---

## 🙏 Remerciements

- Inspiration de [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) par Uncle Bob
- Patterns CQRS de [Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- Communauté Domain-Driven Design
- Tous les contributeurs qui ont participé à ce projet

---

## 📞 Contact & Support

- 📧 Email: support@fleximarket.dev
- 💬 Discord: [Rejoindre la communauté](https://discord.gg/fleximarket)
- 🐛 Issues: [GitHub Issues](https://github.com/votre-username/fleximarket/issues)
- 📚 Documentation: [Wiki](https://github.com/votre-username/fleximarket/wiki)

---

<p align="center">
  Made with ❤️ by the FlexiMarket Team
</p>

<p align="center">
  <a href="#-fleximarket">⬆️ Retour en haut</a>
</p>