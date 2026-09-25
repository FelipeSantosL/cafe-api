O conteúdo 📋

<img width="75" height="20" alt="image" src="https://github.com/user-attachments/assets/3f3b34bc-e68d-419d-ac51-61aad9ee4926" />
<img width="27" height="20" alt="image" src="https://github.com/user-attachments/assets/19a39576-ce33-4841-8cf2-530c3560ec9f" />
<img width="113" height="20" alt="image" src="https://github.com/user-attachments/assets/f622fc80-aed0-4327-92c3-462f735ea5ff" />
<img width="76" height="20" alt="image" src="https://github.com/user-attachments/assets/7d497ff3-a228-44da-b4fc-2b870cfaf5f1" />
<img width="65" height="20" alt="image" src="https://github.com/user-attachments/assets/a82cb83e-14d1-4646-987f-d3dd1167dad1" />
<img width="100" height="20" alt="image" src="https://github.com/user-attachments/assets/6a269c92-70ab-45ca-a006-3475231cfeb7" />

☕ Cafe API

API REST de e-commerce para cafeteria com autenticação, pedidos e controle de estoque — construída em ASP.NET Core com Clean Architecture.

🚧 Projeto de portfólio em evolução — próximo passo: deploy em cloud. Testes de regras de negócio (xUnit + Moq) e containerização (Docker) já implementados.
📌 Funcionalidades
🔐 Autenticação JWT com roles (Admin/Customer) e senhas hasheadas com BCrypt
📦 CRUD de produtos com paginação, busca por nome e filtro por categoria
🛒 Pedidos com múltiplos itens e preço congelado no momento da compra
⚖️ Estoque transacional com concorrência otimista (RowVersion) — sem oversell
🛡️ Isolamento de dados: cada cliente vê apenas os próprios pedidos
⚠️ Erros padronizados em ProblemDetails (RFC 7807) via middleware global
📚 Swagger/OpenAPI com autenticação integrada (botão Authorize)
🏗️ Arquitetura
Clean Architecture em 4 camadas:

Projeto	Responsabilidade
Cafe.Api	Controllers, middleware, configuração
Cafe.Application	DTOs, services, FluentValidation, interfaces
Cafe.Domain	Entidades puras — zero dependências
Cafe.Infrastructure	EF Core, repositórios, JWT, BCrypt
Fluxo de dependência: Api → Application → Domain. A Infrastructure implementa as interfaces da Application — o Domain não conhece ninguém.

🛠️ Stack

ASP.NET Core (Web API) · Entity Framework Core · PostgreSQL 16 · FluentValidation · BCrypt · JWT Bearer · Swashbuckle

🐳 Rodando com Docker
A stack completa (API + PostgreSQL) sobe com um comando:

docker compose up --build
Após o build, acesse: http://localhost:8080/swagger

As migrations são aplicadas com dotnet ef database update --project Cafe.Infrastructure --startup-project Cafe.Api (a connection string padrão já aponta para o container).

⚙️ Como rodar
Com Docker (recomendado)
docker compose up --build# → http://localhost:8080/swagger
Local (sem Docker)
# 1. Clonegit clone https://github.com/FelipeSantosL/cafe-api.gitcd cafe-api
# 2. Atualize a connection string em Cafe.Api/appsettings.json se necessário
# (padrão: Host=localhost;Port=5432;Database=cafe;Username=postgres;Password=cafe123)
# 3. Aplique as migrationsdotnet tool install --global dotnet-ef        
# se ainda não tiverdotnet ef database update --project Cafe.Infrastructure --startup-project Cafe.Api 
# 4. Rode 🚀dotnet run --project Cafe.Api

Primeiro acesso: registre um usuário em POST /api/auth/register. Para torná-lo Admin, execute no banco:

UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'seu-email';

📡 Endpoints
Método	Rota	Descrição	Acesso
POST	/api/auth/register	Criar conta	Público
POST	/api/auth/login	Login (retorna JWT)	Público
GET	/api/products	Listar produtos (paginado)	Público
POST	/api/products	Criar produto	Admin
PUT	/api/products/{id}	Atualizar produto	Admin
POST	/api/orders	Criar pedido	Autenticado
GET	/api/orders/mine	Meus pedidos	Autenticado
GET	/api/orders	Todos os pedidos	Admin
GET	/api/orders/{id}	Detalhe do pedido	Dono ou Admin

🧠 Decisões técnicas
Preço congelado no OrderItem — o cliente envia apenas IDs e quantidades; o preço é sempre consultado no servidor no momento do pedido. Reajustes nunca alteram compras passadas.
Transação única no pedido — validação de estoque, decremento e gravação do pedido acontecem na mesma transação: ou tudo commita, ou nada.
Concorrência otimista — token de versão por linha (xmin do PostgreSQL) garante que dois pedidos simultâneos no mesmo estoque resultem em 409 para um deles.
Anti-enumeração — login inválido sempre responde "Email ou senha inválidos", sem revelar se o email existe; pedidos de outros usuários retornam 404, não 403.
Regras de domínio antes do banco — unicidade e existência de categoria validadas no service, com o banco (FK/unique) como última linha de defesa.
