# Lumière-Vie — README

Jeu Unity (FPS minimal) où tu dois **activer 3 leviers cachés** avant la fin d’un **timer**. Si tu y parviens, la **Lumière-Vie** se stabilise et la boucle s’arrête. Sinon, blackout et retour au début.

---

## Pitch rapide

La ville s’éteint continuellement. Coincé dans cette boucle, tu n’as qu’une chance : **débusquer et enclencher trois leviers** disséminés dans la zone. Chaque levier renforce la Lumière-Vie. Quand les trois sont actifs, la boucle **se brise immédiatement**.

---

## Spécificités du jeu (contrôles)

- **Déplacement** : Z / Q / S / D (ou flèches directionnelles)
- **Regarder** : Souris (contrôle FPS)
- **Interagir** : Clic gauche
- **Pause** : Échap (basculer Pause/Reprendre)
- **Curseur** : Verrouillé en jeu, libre en pause/menu
- **Paramètres** (menu) (Echap):
  - Vitesse de déplacement
  - Sensibilité souris

---

## Objectif du joueur

- **Trouver et activer 3 leviers** dans n’importe quel ordre.
- **Victoire** : dès que **3/3** sont activés (écran de win, timer figé).
- **Défaite** : si le **compte à rebours ** atteint 0 → **fade to black**, réinitialisation, boucle suivante.

---

## Boucle de jeu (gameflow)

1. **Menu Titre** → bouton _Jouer_ démarre la partie, masque le menu, lance le timer.
2. **Exploration** → interagis avec la porte du bus puis active les leviers.
3. **Fin**
   - **Win** si 3/3 leviers actifs (arrêt du temps).
   - **Lose** si timer écoulé : fade → reset positions/états → retour (auto-restart ou titre selon config).

---

## Interactions & états clés

- Les leviers sont des **Interactables** (raycast depuis la caméra).
- Activation d’un levier : son court + changement d’état visuel (si présent).
- `GameController` réagit aux changements et vérifie la **condition de victoire**.
- À la fin de partie, le **Rigidbody du joueur est stoppé** :
  - `rb.velocity = Vector3.zero`
  - `rb.angularVelocity = Vector3.zero`

---

## Technique (aperçu)

- **Timer global** : 420s (unscaled), tick & fin gérés par `GameTimer`.
- **États** : `Idle`, `Running`, `Paused`, `Ended`, `Win`.
- **Events UI** :
  - `OnButtonStartClicked` → `StartGame()`
  - `OnButtonEndClicked` → `ReturnToTitle()`
  - Sliders → vitesse / sensibilité
- **Reset propre** sur défaite : fade noir → reset → fade in (option d’auto-restart).
- **Position de départ** : `START_POSITION = (-1.9, 2, -2.2)`.
