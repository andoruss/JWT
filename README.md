1 - Création de la CI:

à la racine du projet créer un répertoire nomé : 
    
    ".github/workflows"
    
dans ce répertoire ajouter le fichier : 
    
    "ci.yml"

une fois créé ajouter ce contenu :
    
    name: SonarCloud

    on:
      push:
        branches:
          - dev
          - main
      pull_request:
        branches:
          - dev
          - main

    jobs:
      sonarcloud:
        runs-on: ubuntu-latest
        #defini les différentes tâches à exécuter 
        steps:
          - name: Checkout repository
            uses: actions/checkout@v2
    
          - name: SonarCloud Scan
            env:
              SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
              SONAR_PROJECT_KEY: ${{ secrets.SONAR_PROJECT_KEY }}
            run: |
              npm install -g sonarqube-scanner
              sonar-scanner \
                -Dsonar.projectKey=${{ secrets.SONAR_PROJECT_KEY }} \
                -Dsonar.organization=andoruss \
                -Dsonar.sources=. \
                -Dsonar.host.url=https://sonarcloud.io \
                -Dsonar.login=${{ secrets.SONAR_TOKEN }}

Les variables "SONAR_TOKEN" et "SONAR_PROJECT_KEY" son défini dans l'onglet Settings -> Secret and variable -> Action.
Créer deux nouvelles varaibles de repository qui on le même nome que celle dans le fichier de la ci.
Pour "SONAR_TOKEN" est égal au token généré quand le projet est créé sur sonarcloud alors que "SONAR_PROJECT_KEY" est la clé que l'on retrouve sur le projet sonnarcloud dans l'onglet information en bas à gauhce. L'organisation correspond au nom de l'oraganisation dans lequel le projet sonarCloud se trouve.

Lorsque la ci est lancé on peut voir le résultat du scan sur le projet sonarCloud.

2- Création de la base de données et de la migration: 

Installer dans la couche context et dans l'api les packages nugets ->
    
    Microsoft.EntityFrameworkCore
    Microsoft.EntityFrameworkCore.Design
    Microsoft.EntityFrameworkCore.SqlServer
    Microsoft.EntityFrameworkCore.Tools

Ajouter un fichier context dans la couche context ->

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {    
        public DbSet<Garage> Garages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<User> Users { get; set; }
    }

Ajout de la connexion string pour se connecter à la base de données dans les fichier settings de l'api->
Il faut au préalable créer une base de données vide.

    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=LearnJWT;Trusted_Connection=True;"
    }

Migration des entités dans la base de données -> 
aller dans le gestionnair de package et sélectionner le projet par default qui contient le dbContext

ajouter une migration :

    add-migration InitialMigration
    
mettre la base à jour :

    update-database --verbose

    

Remplissage de la base de fausses données:

Hachage des mot de passe

Création d'un JWT 

Ajout des authorisations sur les routes

