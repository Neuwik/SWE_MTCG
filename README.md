# SWE_MTCG (C#)
Dominik Neuwirth
if22b022

# Git Repository Link
https://github.com/Neuwik/SWE_MTCG.git


# Setup Docker DB:
docker run --name SWE_MTCG -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=debian123! -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres


# Curl Script Changes: (edited.curl.withpause.bat)
ID -> intager  
Name -> Username  
2 -> should fail added when already logged in  
2.1 -> Logout added  
3,6 -> useless because card packs are random and not created  
5 -> should fail deleted because no limeted amount of card packs  
7 -> less money because more spent on 5  
14 -> username in path after change  
14 -> added calls for admin access  
14 -> added should fail calls for username exists  
19.1 -> added some extra battles for the score board  
19.2 -> some delete battle added (leave que)  