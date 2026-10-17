# 📱 Mobile Port System

Sistema para porte de jogos para mobile.

---

## Como funciona

O sistema fornece um canvas pré-configurado que é ativado ou desativado conforme a plataforma em que o jogo está rodando. Esse canvas contém botões **On-Screen** que permitem ao jogador realizar ações como andar e pular sem um controle físico.

## Como usar

1. Adicione o prefab **Mobile Port System** a, **pelo menos**, a primeira cena do jogo. O objeto usa `DontDestroyOnLoad` e persiste durante toda a sessão.
2. Adicione o prefab **Mobile Port System UI** ao Canvas das cenas em que o porte faz sentido. Menus não precisam, pois já funcionam nativamente com touch.
3. Acesse o sistema externamente, via glue code ou referência direta, para ativar ou desativar a UI quando necessário.

## Pré-requisitos

- **Input System** instalado no projeto.
- Um **Input Mapping** com as ações do jogo já configuradas.

## Configuração

A UI deve acompanhar os inputs do jogo. Se novas ações forem adicionadas, é necessário adaptar a UI do sistema para acomodá-las.

Cada botão deve ter o componente **On-Screen Button**, com o caminho da ação definido em **Control Path**. As ações precisam estar pré-configuradas no Input Mapping usado pelo jogo.

### `Port UI`

Script responsável por lidar com os elementos da UI do sistema.

Novos botões criados devem ser adicionados à lista `disableableOnPause`, para que sejam desativados quando o menu de pause abrir, se houver.

| Campo | Descrição |
|---|---|
| `usesStick` | Deve ser configurado de acordo com a movimentação do jogo. Quando ativado, desabilita automaticamente os botões de movimento; quando desativado, desabilita o on-screen joystick. |