O conteúdo 📋

☕ Cafe API
API REST de e-commerce para cafeteria com autenticação, pedidos e controle de estoque — construída em ASP.NET Core com Clean Architecture.

.NETC#SQL ServerEF Core

🚧 Projeto de portfólio em evolução — próximos passos: testes automatizados (xUnit) e deploy.
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

ASP.NET Core (Web API) · Entity Framework Core · SQL Server (LocalDB) · FluentValidation · BCrypt · JWT Bearer · Swashbuckle

⚙️ Como rodar
Pré-requisitos: .NET 10 SDK e SQL Server LocalDB (incluso no Visual Studio).

# 1. Clonegit clone https://github.com/FelipeSantosL/cafe-api.gitcd cafe-api
# 2. Aplique as migrations (cria o banco com dados de exemplo)dotnet tool install --global dotnet-ef      
# se ainda não tiver dotnet ef database update --project Cafe.Infrastructure --startup-project Cafe.Api
# 3. Rode 🚀dotnet run --project Cafe.Api
# 4. Abra o Swagger# https://localhost:PORTA/swagger  (a porta aparece no console)
A connection string padrão aponta para LocalDB — ajuste em Cafe.Api/appsettings.json para usar outro SQL Server.

Primeiro acesso: registre um usuário em POST /api/auth/register. Para torná-lo Admin, execute no banco:

UPDATE Users SET Role = 'Admin' WHERE Email = 'seu-email';

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
Concorrência otimista — RowVersion nos produtos garante que dois pedidos simultâneos no mesmo estoque resultem em 409 para um deles.
Anti-enumeração — login inválido sempre responde "Email ou senha inválidos", sem revelar se o email existe; pedidos de outros usuários retornam 404, não 403.
Regras de domínio antes do banco — unicidade e existência de categoria validadas no service, com o banco (FK/unique) como última linha de defesa.

🚧 Projeto de portfólio em evolução — próximo passo: deploy em cloud. Testes de regras de negócio com xUnit + Moq já implementados.
