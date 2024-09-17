document.addEventListener("DOMContentLoaded", function () {
    var observer = new MutationObserver(function (mutations) {
        var topbarWrapper = document.querySelector(".swagger-ui .topbar .topbar-wrapper");
        if (topbarWrapper) {
            // Находим элемент логотипа
            var logo = topbarWrapper.querySelector(".link svg");

            if (logo) {
                // Скрываем логотип
                logo.remove();

                // Создаем новый элемент для надписи
                var newHeaderText = document.createElement("span");
                newHeaderText.innerHTML = "WebSchedulingSystem API";
                newHeaderText.style.color = "white"; // Цвет текста
                newHeaderText.style.textAlign = "center"; // Выравнивание текста
                newHeaderText.style.fontSize = "16px"; // Размер текста
                newHeaderText.style.fontWeight = "bold"; // Жирность текста
                newHeaderText.style.marginLeft = "10px"; // Отступ слева
                newHeaderText.style.display = "inline-block"; // Отображение inline-block
                newHeaderText.style.width = "300px"; // Увеличение ширины

                // Добавляем новый элемент в .link
                var linkElement = topbarWrapper.querySelector(".link");
                if (linkElement) {
                    linkElement.appendChild(newHeaderText);
                }

                observer.disconnect(); // Отключаем наблюдателя после изменения элемента
            }
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
});
