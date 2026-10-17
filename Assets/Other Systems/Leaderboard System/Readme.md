# 🏆 Leaderboard System

Asset para fácil implementação de uma tabela de classificação em jogos que possuem sistema de pontuação.

---

## Como funciona

O sistema fornece uma cena com a leaderboard já montada, decorada e pronta para uso.

Ele utiliza o pacote **Leaderboards** da Unity e se conecta à **Unity Cloud**, de modo que os registros da tabela ficam salvos na nuvem.

## Como usar

1. Adicione a cena **Leaderboard** à Scene List do Build Settings.
2. Conecte o projeto à Unity Cloud com sua conta da Unity ([veja como](#conectando-à-unity-cloud)).
3. No dashboard da Unity, crie uma nova leaderboard para o projeto.
4. Preencha o **General Data** com o ID da leaderboard criada.
5. No sistema do jogo responsável pela pontuação, salve o valor no `PlayerPrefs` com a chave **`Score`**.

## Pré-requisitos

- Uma **Leaderboard** criada na Unity Cloud.
- O pacote **Leaderboards** instalado no projeto.

## Conectando à Unity Cloud

1. Em **Edit → Project Settings → Services**, vincule o projeto a uma organização e a um projeto da Unity Cloud.
2. No [dashboard da Unity](https://cloud.unity.com), acesse o projeto do jogo, vá em **Products → Leaderboards** e crie uma nova leaderboard.
3. Copie o **Leaderboard ID** e cole no campo correspondente do **General Data**.

> Algumas configurações de como a tabela funciona são definidos no dashboard da Unity, não no projeto.

## Configuração

As opções do sistema são definidas pelos Scriptable Objects **`General Data`** e **`Audio Data`**, localizados na pasta **`_Data`**.

### `General Data`

Scriptable Object com a maioria das configurações do sistema. Contém os valores dos efeitos únicos e os parâmetros usados pelos diversos scripts.

| Campo | Descrição |
|---|---|
| `loadNextScene` | Se ativado, o sistema carrega sozinho a cena de nome `nextSceneName` ao sair da leaderboard. |
| `nextSceneName` | Nome da cena a ser carregada na saída. |
| `evaluatesResult` | Ativa ou desativa o `Result Evaluator`. |

Se `loadNextScene` estiver desativado, o sistema emite o sinal **`OnLeaderboardExitted`**, para que um sistema próprio de transição de cenas faça a troca.

### `Result Evaluator`

Script responsável por definir a imagem e a classificação obtidas a partir da pontuação final do jogador.

As imagens e os tiers são definidos no **General Data**. Recomenda-se usar os mesmos tiers configurados na leaderboard da Unity Cloud. Se o jogo não precisar dessa funcionalidade, basta desativar **`evaluatesResult`**.

### `Effects Controller`

Script auxiliar que organiza os efeitos chamados por cada funcionalidade.

Os efeitos de **General Effects** podem ser adicionados com os prefabs prontos. Recomenda-se que sejam filhos do objeto que contém o `Effects Controller` e que os parâmetros únicos, se houver, sejam definidos no **General Data**.