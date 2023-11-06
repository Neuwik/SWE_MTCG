# SWE_MTCG

docker run --name SWE_MTCG -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=debian123! -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres

docker run -d --name pgadmin4-client -e PGADMIN_DEFAULT_EMAIL=user@domain.com -e PGADMIN_DEFAULT_PASSWORD=your_password -p 5050:80 dpage/pgadmin4

